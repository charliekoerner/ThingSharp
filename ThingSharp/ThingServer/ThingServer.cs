using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ThingSharp.Container;
using ThingSharp.Types;

namespace ThingSharp.Server
{
    public class ThingServer : IBindingClient
    {
        private bool _Status;
        private ProtocolBinding mBinding;
        private ThingContainer mThings;
        private Adapter mAdapter;
        public ThingServer(ProtocolBinding binding, Adapter adapter)
        {
            _Status = true;
            mBinding = binding;
            mBinding.AddClient(this);
            mThings = new ThingContainer();
            mAdapter = adapter;
            mAdapter.ThingAdded += (t) => { this.ResourceContainer.Add(t); };
        }

        public void Start()
        {
            mBinding.StartListening();
        }

        public void Stop()
        {
            mAdapter.Stop();
            mBinding.StopListening();
        }

        public void SetStatus(bool status)
        {
            _Status = status;
        }

        private List<HypermediaLink> GetThingLinks()
        {
            List<HypermediaLink> thingUris = new List<HypermediaLink>();
            foreach (Resource r in mThings)
            {
                thingUris.Add(new HypermediaLink() { uri = r.Uri, rel = "td" });
            }
            return thingUris;
        }

        public object Read(Uri uri)
        {
            if (uri.AbsolutePath == "/")
            {
                return GetThingLinks();
            }
            else if(uri.AbsolutePath.ToLower() == "/status")
            {
                return _Status;
            }

            Resource resource = mThings.ResolveUrl(uri);

            if (resource != null)
                try
                {
                    return mAdapter.Read(resource);
                }
                catch (Exception)
                {
                    throw new Resource.ResourceOperationFailedException();
                }
            else
            {
                return null;
                //throw new Resource.ResourceNotFoundException();
            }
        }


        public object Write(Uri uri, object value)
        {
            Resource resource = mThings.ResolveUrl(uri);

            if (resource is Thing)
            {
                throw new Resource.ResourceOperationNotAllowedException();
            }
            if (resource != null)
            {
                try
                {
                    return mAdapter.Write(resource, value);
                }
                catch (Exception e)
                {
                    throw new Resource.ResourceOperationFailedException();
                }
            }
            else
            {
                return null;
                //throw new Resource.ResourceNotFoundException() { Message = "Resource not found" };
            }
        }

        public ThingContainer ResourceContainer
        {
            get { return mThings; }
        }
    }
}

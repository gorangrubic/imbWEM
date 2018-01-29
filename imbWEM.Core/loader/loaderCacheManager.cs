using imbACE.Core.cache;
using imbSCI.Core.files.folders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace imbWEM.Core.loader
{

    

    public class loaderCacheManager : cacheSimpleBase<loaderRequest>
    {
        public loaderCacheManager(folderNode node, Int32 hoursLimit):base(node, hoursLimit, "wbi")
        {

        }

        public override string getInstanceID(object instance)
        {
            loaderRequest lr = instance as loaderRequest;
            return lr.url;
        }
    }
}

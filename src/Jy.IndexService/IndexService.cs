using System;
using Jy.IIndex; 

namespace Jy.IndexService
{
    public class IndexService : IIndexService
    {
        private IIndexReadFactory _indexReadFactory;
        public IIndexReadFactory IndexReadFactory { set { _indexReadFactory = value; } get { return _indexReadFactory; } }

        private IIndexFactory _indexFactory;
        public IIndexFactory IndexFactory { set { _indexFactory = value; } get { return _indexFactory; } }
        public IndexService(IIndexFactory indexFactory, IIndexReadFactory indexReadFactory)
        {
            _indexReadFactory = indexReadFactory;
            _indexFactory = indexFactory;
        }
    }
}

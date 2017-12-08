using System;
using Jy.IIndex;

namespace Jy.IndexService
{
    public class IndexService : IIndexService
    {
        public IIndexRead indexRead { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IIndex.IIndex index { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

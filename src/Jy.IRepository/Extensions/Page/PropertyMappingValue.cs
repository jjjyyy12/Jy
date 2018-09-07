using System.Collections.Generic;

namespace Jy.IRepositories
{
    public class PropertyMappingValue
    {
        public IEnumerable<string> DestinationProperties { get; }
        public bool Revert { get; }

        public PropertyMappingValue(IEnumerable<string> destinationProperties, bool revert = false)
        {
            DestinationProperties = destinationProperties;
            Revert = revert;
        }
    }
}

using System.Collections.Generic;
using PG.Asteroids.Models.DataModels;

namespace PG.Asteroids.Models
{
    public class StaticDataModel
    {
        public MetaData MetaData;

        public void SeedMetaData(MetaData metaData)
        {
            MetaData = metaData;
        }
    }
}


using System.Collections;
using System.Collections.Generic;

namespace GeoParam {
    using LSystem;
    public abstract class IGeoEmbedder {
        // temp solution, we should use portfolio instead of LString here
        public abstract List<List<IGeoParam>> geoEmbbed(ILString lString, bool backward);
        public abstract List<IGeoParam> charGeoEmbbed(LGS.LGSCollisionData current, LGS.LGSCollisionData next, bool backward, bool random);
    }
}
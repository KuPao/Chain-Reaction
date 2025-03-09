using System.Collections;
using System.Collections.Generic;

namespace GeoParam {
    public abstract class IGeoParam {
        public GeoParamType type;

        public IGeoParam(GeoParamType type) {
            this.type = type;
        }
    }
}

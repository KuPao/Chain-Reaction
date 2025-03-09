using System.Collections;
using System.Collections.Generic;

namespace LSystem {
    public abstract class ILSystem {
        public ILString lString;

        public ILSystem() {
            lString = getAxiom();
        }

        public abstract ILString getAxiom();
        public abstract List<ILString> getExtendCandidates();
    }
}


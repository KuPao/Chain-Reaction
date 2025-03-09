using System.Collections;
using System.Collections.Generic;

namespace LSystem {
    public abstract class ISymbol {
        public char alphabet;

        public ISymbol(char c) {
            alphabet = c;
        }
    }
}

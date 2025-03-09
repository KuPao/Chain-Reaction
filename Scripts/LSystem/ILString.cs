using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem {
    public abstract class ILString {
        public List<ISymbol> symbols;

        public ILString() {
            symbols = new List<ISymbol>();
        }
        public ILString(ISymbol[] symbols) {
            this.symbols = new List<ISymbol>(symbols);
        }
        public ILString(List<ISymbol> symbols) {
            this.symbols = new List<ISymbol>(symbols);
        }

        public string toString() {
            string s = "";
            for (int i = 0; i < symbols.Count; i++) {
                s += symbols[i].alphabet;
            }
            return s;
        }
    }
}

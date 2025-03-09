using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LSystem {
    public class CR_Symbol : ISymbol {
        public CR_Symbol(char c) : base(c) {
        }
    }
    public class CR_LString : ILString {
        public CR_LString() {
            symbols = new List<ISymbol>();
        }
        public CR_LString(ISymbol[] symbols) : base(symbols) {
        }
        public CR_LString(List<ISymbol> symbols) : base(symbols) {
        }
    }


    public class CR_LSystem : ILSystem {
        static public CR_Symbol Ball_Symbol = new CR_Symbol('B');
        static public CR_Symbol Dominos_Symbol = new CR_Symbol('D');
        static public CR_Symbol Pulley_Symbol = new CR_Symbol('P');
        static public CR_Symbol Cup_Symbol = new CR_Symbol('C');
        static public CR_Symbol Billiard_Symbol = new CR_Symbol('L');
        static public CR_Symbol Branch_Start_Symbol = new CR_Symbol('[');
        static public CR_Symbol Branch_End_Symbol = new CR_Symbol(']');

        public CR_LSystem() {
        }

        public override ILString getAxiom() {
            return new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol, CR_LSystem.Cup_Symbol });
            //300
            //return new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol,
            //    CR_LSystem.Ball_Symbol, CR_LSystem.Billiard_Symbol, CR_LSystem.Branch_Start_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol, 
            //    CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Cup_Symbol, CR_LSystem.Branch_End_Symbol,
            //    CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol,
            //    CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Billiard_Symbol, CR_LSystem.Branch_Start_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, 
            //    CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, 
            //    CR_LSystem.Cup_Symbol, CR_LSystem.Branch_End_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Dominos_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, 
            //    CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Cup_Symbol });
        }
        public ILString stringToLString(string str) {
            List<ISymbol> symbols = new List<ISymbol>();
            for (int i = 0; i < str.Length; i++) {
                switch (str[i]) {
                    case 'B':
                        symbols.Add(CR_LSystem.Ball_Symbol);
                        break;
                    case 'D':
                        symbols.Add(CR_LSystem.Dominos_Symbol);
                        break;
                    case 'P':
                        symbols.Add(CR_LSystem.Pulley_Symbol);
                        break;
                    case 'C':
                        symbols.Add(CR_LSystem.Cup_Symbol);
                        break;
                    case 'L':
                        symbols.Add(CR_LSystem.Billiard_Symbol);
                        break;
                    case '[':
                        symbols.Add(CR_LSystem.Branch_Start_Symbol);
                        break;
                    case ']':
                        symbols.Add(CR_LSystem.Branch_End_Symbol);
                        break;
                }
            }
            return new CR_LString(symbols);
        }
        public override List<ILString> getExtendCandidates() {
            List<ILString> candidates = new List<ILString>();

            for(int i = 0; i < lString.symbols.Count; i++) {
                List<CR_LString> insertContents;
                if (i == 0) {
                    insertContents = insertAtFirst(lString.symbols[i]);
                }else {
                    insertContents = insertAtMiddle(lString.symbols[i - 1], lString.symbols[i], i);
                }
                for (int j = 0; j < insertContents.Count; j++) {
                    List<ISymbol> symbols = new List<ISymbol>();
                    for (int k = 0; k < i; k++) {
                        symbols.Add(lString.symbols[k]);
                    }
                    for (int k = 0; k < insertContents[j].symbols.Count; k++) {
                        symbols.Add(insertContents[j].symbols[k]);
                    }
                    for (int k = i; k < lString.symbols.Count; k++) {
                        symbols.Add(lString.symbols[k]);
                    }
                    candidates.Add(new CR_LString(symbols));
                }
            }
            
            return candidates;
        }

        private List<CR_LString> insertAtFirst(ISymbol firstSymbol) {
            List<CR_LString> rt = new List<CR_LString>();
            if (firstSymbol.alphabet == 'B') {
                // won't happend
                //rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol }));
            }
            else if(firstSymbol.alphabet == 'D') {
                rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol }));
            }
            else if(firstSymbol.alphabet == 'P') {
                rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol }));
                rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
            }
            else if (firstSymbol.alphabet == 'C') {
                rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol }));
                rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
            }
            
            return rt;
        }

        private List<CR_LString> insertAtMiddle(ISymbol prevSymbol, ISymbol nextSymbol, int i) {
            List<CR_LString> rt = new List<CR_LString>();
            if (prevSymbol.alphabet == 'B') {
                if (nextSymbol.alphabet == 'B') {
                    // won't happen
                }
                else if(nextSymbol.alphabet == 'D') {
                    //rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Billiard_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
                else if (nextSymbol.alphabet == 'P') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                    if (!insideBranch(i - 1)) {
                        rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Billiard_Symbol, CR_LSystem.Branch_Start_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Cup_Symbol, CR_LSystem.Branch_End_Symbol }));
                    }
                }
                else if (nextSymbol.alphabet == 'C') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    //rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Billiard_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
            }
            else if (prevSymbol.alphabet == 'D') {
                if (nextSymbol.alphabet == 'B') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'D') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                }
                else if (nextSymbol.alphabet == 'P') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
                else if (nextSymbol.alphabet == 'L') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'C') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
            }
            else if (prevSymbol.alphabet == 'P') {
                if (nextSymbol.alphabet == 'B') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Ball_Symbol, CR_LSystem.Pulley_Symbol }));
                }
                else if (nextSymbol.alphabet == 'D') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'P') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'L') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'C') {
                    // won't happen
                }
            }
            else if (prevSymbol.alphabet == 'L') {
                if (nextSymbol.alphabet == 'B') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'D') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
                else if (nextSymbol.alphabet == 'P') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
                else if (nextSymbol.alphabet == 'C') {
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Dominos_Symbol }));
                    rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol }));
                }
            }
            else if (prevSymbol.alphabet == ']') {
                if (nextSymbol.alphabet == 'B') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'D') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'P') {
                    if (!secondBranch(i - 1)) {
                        rt.Add(new CR_LString(new ISymbol[] { CR_LSystem.Branch_Start_Symbol, CR_LSystem.Pulley_Symbol, CR_LSystem.Ball_Symbol, CR_LSystem.Cup_Symbol, CR_LSystem.Branch_End_Symbol }));
                    }
                }
                else if (nextSymbol.alphabet == 'L') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'C') {
                    // won't happen
                }
            }
            else if (prevSymbol.alphabet == 'C') {
                if (nextSymbol.alphabet == 'B') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'D') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'P') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'L') {
                    // won't happen
                }
                else if (nextSymbol.alphabet == 'C') {
                    // won't happen
                }
            }

            return rt;
        }

        private bool insideBranch(int index) {
            for (int i = index-1; i >= 0; i--) {
                if (lString.symbols[i].alphabet == '[') {
                    return true;
                }
                else if (lString.symbols[i].alphabet == ']') {
                    return false;
                }
            }
            return false;
        }
        private bool secondBranch(int index) {
            for (int i = index - 1; i >= 0; i--) {
                if (lString.symbols[i].alphabet == '[') {
                    if (lString.symbols[i - 1].alphabet == ']') {
                        return true;
                    }
                    else {
                        return false;
                    }
                }
            }
            return false;
        }
    }
}
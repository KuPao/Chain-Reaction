using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

namespace LGS
{
    public class LGSSolution
    {
        public string name = "";
        public LSystem.ILString lstring;

        public int mode = 1;
        public int time = 30;
        public int shot = 4;

        public List<LGSCollisionData> collisions = new List<LGSCollisionData>();
        public Dictionary<int, List<LGSCollisionData>> pathCollisions = new Dictionary<int, List<LGSCollisionData>>();
        public Dictionary<int, List<LGSCollisionData>> ballCollisions = new Dictionary<int, List<LGSCollisionData>>();
        public Dictionary<int, List<LGSCollisionData>> layerCollisions = new Dictionary<int, List<LGSCollisionData>>();

        public float length = 0;
        public float difficulty;

        public int side = -1;

        public float sidingY = 0.0f;
        public float scalar = 1.0f;

        public List<LGSSolution> children = new List<LGSSolution>();
        public LGSSolution parent = null;
        public CR_LMCTS.MCTSNode root = null;
        public static int total = 0;
        public int n = 0;
        public float w = 0.0f;
        public float fitness;
        public float high_score = 0.0f;
        public float avg_score = 0.0f;
        public float total_scroe = 0.0f;
        public int counter = 0;
        public int maxLayer = 0;

        public Vector3 aimDir
        {
            get
            {
                var cbPath = ballCollisions[0];
                return (cbPath[1].position - cbPath[0].position).normalized;
            }
        }

        //public void Draw(string lineParent)
        //{
        //    foreach (var collision in collisions)
        //    {
        //        collision.Draw(lineParent);
        //    }
        //}

        public LGSSolution(string sentence, CR_LMCTS lmcts)
        {
            //var stack = new Stack<LGSCollisionData>();

            //int ballID = 0;

            //LGSCollisionData last = null;

            //bool backward = true;

            //foreach (var c in sentence)
            //{

            //    LGSCollisionData curr = null;

            //    switch (c)
            //    {
            //        case '[':
            //            stack.Push(last);
            //            backward = false;
            //            break;
            //        case ']':
            //            last = stack.Pop();
            //            backward = true;
            //            break;
            //        default:
            //            curr = new LGSCollisionData(c, ballID);
            //            break;
            //    }

            //    // c is a unit
            //    if (curr != null)
            //    {
            //        // c is not first char
            //        if (last != null)
            //        {
            //            curr.layer = last.layer;

            //            if (c == 'I')
            //            {
            //                last.targ = curr;
            //                curr.targ = last;
            //            }
            //            else
            //            {
            //                curr.prev = last;
            //                last.next = curr;

            //                curr.layer++;
            //            }
            //        }

            //        // previous collision data
            //        curr.list_prev = last;

            //        // if not first, add current data to next of last data
            //        if (last != null)
            //            last.list_next = curr;
            //        curr.backward = backward;
            //        // update last data
            //        last = curr;

            //        // add a list if not exist in dict
            //        if (!ballCollisions.ContainsKey(curr.ballID))
            //            ballCollisions.Add(curr.ballID, new List<LGSCollisionData>());

            //        if (!layerCollisions.ContainsKey(curr.layer))
            //            layerCollisions.Add(curr.layer, new List<LGSCollisionData>());

            //        ballCollisions[curr.ballID].Add(curr);
            //        layerCollisions[curr.layer].Add(curr);

            //        collisions.Add(curr);
            //    }
            //}

            //last = null;

            //int id = 0;

            //foreach (var curr in collisions)    
            //{
            //    curr.id = id++;

            //    if (last != null)
            //        last.list_next = curr;

            //    curr.list_prev = last;

            //    last = curr;
            //}

            //GetPathCollisions();
            var stack = new Stack<LGSCollisionData>();
            var orderStack = new Stack<LGSCollisionData>();

            int ballID = 0;

            LGSCollisionData last = null;
            LGSCollisionData orderLast = null;
            LGSCollisionData orderTemp = null;

            bool backward = true;
            int id = 0;
            foreach (var c in sentence) {

                LGSCollisionData curr = null;

                switch (c) {
                    case '[':
                        stack.Push(last);
                        if (orderTemp != null) {
                            orderStack.Push(orderTemp);
                        }
                        else {
                            orderStack.Push(orderLast);
                        }
                        backward = false;
                        break;
                    case ']':
                        last = stack.Pop();
                        orderTemp = orderStack.Pop();
                        backward = true;
                        break;
                    default:
                        if (orderStack.Count == 0 && orderTemp != null) {
                            orderLast.list_next = orderTemp.list_prev;
                            orderTemp.list_prev.list_next = orderLast;
                            orderLast = orderTemp;
                            orderTemp = null;
                        }
                        curr = new LGSCollisionData(c, ballID);
                        curr.id = id;
                        break;
                }
                id++;

                //c is a unit
                if (curr != null) {
                    // c is not first char
                    if (orderLast != null) {
                        if (c == 'I') {
                            last.targ = curr;
                            curr.targ = last;
                        }
                        else {
                            curr.list_prev = orderLast;
                            orderLast.list_next = curr;
                        }
                    }

                    // previous collision data
                    curr.prev = last;

                    // if not first, add current data to next of last data
                    if (last != null) {
                        curr.layer = last.layer;
                        last.next = curr;
                        curr.layer++;
                        if (curr.layer > maxLayer) {
                            maxLayer = curr.layer;
                        }
                    }
                        
                    curr.backward = backward;
                    // update last data
                    orderLast = curr;
                    last = curr;

                    // add a list if not exist in dict
                    if (!ballCollisions.ContainsKey(curr.ballID))
                        ballCollisions.Add(curr.ballID, new List<LGSCollisionData>());

                    if (!layerCollisions.ContainsKey(curr.layer))
                        layerCollisions.Add(curr.layer, new List<LGSCollisionData>());

                    ballCollisions[curr.ballID].Add(curr);
                    layerCollisions[curr.layer].Add(curr);

                    collisions.Add(curr);
                }
            }

            last = null;

            

            foreach (var curr in collisions) {
                
                if (curr.backward == false) {
                    var tmp = curr.prev;
                    curr.prev = curr.next;
                    curr.next = tmp;
                }
            }
            last = null;
            for (int i = collisions.Count - 1; i >= 0; i--) {
                LGSCollisionData curr = collisions[i];
                if(curr.backward == false) {
                    var tmp = curr.list_prev;
                    curr.list_prev = curr.list_next;
                    curr.list_next = tmp;
                    last = curr;
                }
                else if (curr.elementType == LGSCollisionData.ElementType.L) {
                    curr.list_prev = last;
                }
            }

            //GetPathCollisions();
            // genRandomGeoParams?
            //Init(lmcts);
        }

        public void printSolution() {
            for(int i = 0; i < collisions.Count; i++) {
                LGSCollisionData c = collisions[i];
                if(c.list_prev == null) {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " null " + c.list_next.elementType.ToString() + c.list_next.id.ToString());
                }
                else if(c.list_next == null) {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " " + c.list_prev.elementType.ToString() + c.list_prev.id.ToString() + " null");
                }
                else {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " " + c.list_prev.elementType.ToString() + c.list_prev.id.ToString() + " " + c.list_next.elementType.ToString() + c.list_next.id.ToString());
                }
                Debug.Log(c.layer);
            }
            for (int i = 0; i < collisions.Count; i++) {
                LGSCollisionData c = collisions[i];
                if (c.prev == null) {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " null " + c.next.elementType.ToString() + c.next.id.ToString());
                }
                else if (c.next == null) {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " " + c.prev.elementType.ToString() + c.prev.id.ToString() + " null");
                }
                else {
                    Debug.Log(c.elementType.ToString() + c.id.ToString() + " " + c.prev.elementType.ToString() + c.prev.id.ToString() + " " + c.next.elementType.ToString() + c.next.id.ToString());
                }
                Debug.Log(c.layer);
            }
        }

        public void Backpropagation(float w) {
            this.n += 1;
            this.w += w;

            if (parent != null)
                parent.Backpropagation(w);
        }

        public List<LGSSolution> GetChildSolution(CR_LMCTS lmcts) {
            List<LSystem.ILString> candidates = lmcts.lSystem.getExtendCandidates();
            for (int i = 0; i < candidates.Count; i++) {
                for (int j = i + 1; j < candidates.Count; j++) {
                    if (candidates[i].toString() == candidates[j].toString()) {
                        candidates.RemoveAt(j--);
                    }
                }
            }
            foreach (var candidate in candidates) {
                LGSSolution newSolution = new LGSSolution(candidate.toString(), lmcts);
                newSolution.lstring = candidate;
                newSolution.parent = lmcts.root.solution;
                children.Add(newSolution);
            }
            return children;
        }

        public List<List<LGSSolution>> genChildren(CR_LMCTS lmcts, int layer) {
            List<List<LGSSolution>> layerList = new List<List<LGSSolution>>();
            List<LGSSolution> list = this.SelectList();
            layerList.Add(new List<LGSSolution>());
            layerList[0].AddRange(list);
            int num = 0;
            for (int i = 0; i < layer; i++) {
                List<LGSSolution> newChildren = new List<LGSSolution>();
                if (i < 6) {
                    for (int j = 0; j < list.Count; j++) {
                        lmcts.lSystem.lString = list[j].lstring;
                        newChildren.AddRange(list[j].GetChildSolution(lmcts));
                    }
                    if (i == 5) {
                        num = list.Count;
                    }
                }
                else {
                    num = 5 * i * Random.Range(i, layer);
                    for (int j = 0; j < num; j++) {
                        int index = Random.Range(0, list.Count);
                        lmcts.lSystem.lString = list[index].lstring;
                        newChildren.AddRange(list[index].GetChildSolution(lmcts));
                        list.RemoveAt(index);
                    }
                }
                
                list = new List<LGSSolution>();
                layerList.Add(newChildren);
                //Debug.Log(layerList[i + 1].Count);
                list.AddRange(newChildren);
            }
            return layerList;
        }
        public void genChildrenNoRepeat(CR_LMCTS lmcts, int layer) {
            List<LGSSolution> list = this.SelectList();
            for (int i = 0; i < layer; i++) {
                List<LGSSolution> newChildren = new List<LGSSolution>();
                for (int j = 0; j < list.Count; j++) {
                    lmcts.lSystem.lString = list[j].lstring;
                    newChildren.AddRange(list[j].GetChildSolution(lmcts));
                }
                for (int j = 0; j < newChildren.Count; j++) {
                    for (int k = j + 1; k < newChildren.Count; k++) {
                        if (newChildren[j].lstring.toString() == newChildren[k].lstring.toString()) {
                            newChildren.RemoveAt(k);
                        }
                    }
                }
                list = new List<LGSSolution>();
                list.AddRange(newChildren);
            }
        }

        public List<LGSSolution> SelectList() {
            var result = children.SelectMany(i => i.SelectList()).ToList();

            result.Add(this);

            return result.ToList();
        }

        public LGSSolution Select(float maxScore) {
            if (children.Count > 0) {
                LGSSolution chosen = null;

                foreach (var child in children) {
                    if (child.n != 0) {
                        var score = child.high_score;

                        if (score > maxScore) {
                            maxScore = score;
                            chosen = child;
                        }

                    }
                    else {
                        chosen = child;
                        break;
                    }

                }
                
                if (chosen == null) {
                    return this;
                }
                else {
                    return chosen.Select(maxScore);
                }
            }
            else {
                return this;
            }
        }

        public float Score() {
            if (parent == null) {
                return w / n + Mathf.Sqrt(2 * Mathf.Log(n) / n);
            }
            return w / n + Mathf.Sqrt(2 * Mathf.Log(parent.n) / n);
            //return w / n + Mathf.Sqrt(2 * Mathf.Log(total) / n);
        }

        public void GetPathCollisions()
        {
            foreach (var pair in ballCollisions)
            {
                var collision = pair.Value.Last();

                LGSCollisionData next = null;

                var collisionsList = new List<LGSCollisionData>();

                pathCollisions.Add(collision.ballID, collisionsList);

                while (collision != null)
                {
                    collisionsList.Add(collision);

                    next = collision;

                    // 換下一輪
                    if (collision.prev == null)
                        collision = collision.targ;
                    else
                        collision = collision.prev;
                }
            }
        }

        public void Init(CR_LMCTS lmcts)
        {
            lmcts.genRandomGeoParams();
            for (int i = 0; i < collisions.Count; i++) {
                collisions[i].Init(lmcts.geoParams[i]);
            }
        }

        //計算難度
        public float Fitness(float targetTime, float time, bool successful)
        {
            if (Update(false) == false)
            {
                return 0.0f;
            }

            GetLength();
            return .0f;
        }

        public void GetLength()
        {
            length = 0.0f;
            foreach (var collision in collisions)
            {
                if (collision.onTail == false)
                {
                    collision.Calculate();
                    length += collision.length;
                }
            }
        }

        public bool Update(bool calculate = true)
        {
            bool success = true;

            foreach (var collision in collisions)
            {
                if (collision.Update() == false)
                {
                    return false;
                }
                //success &= collision.Update();
            }

            /*if (calculate)
                Calculate();*/

            return success;
        }

        public void Calculate()
        {
            RBMScore();

            foreach (var collision in collisions)
            {
                difficulty += collision.difficulty;
                length += collision.length;

                collision.minAngle = -10000.0f;
                collision.maxAngle = 10000.0f;
            }

            //Debug.Log("難度為"+ difficulty);
            //Debug.Log("長度為" + length);
            foreach (var pair in ballCollisions)
            {
                var collision = pair.Value.Last();

                LGSCollisionData next = null;

                if (collision.elementType != LGSCollisionData.ElementType.C)
                {
                    while (collision != null)
                    {
                        //容錯角度計算  打磚塊不牽扯到複雜物理引擎
                        //collision.DifficultyAngle(next);
                        next = collision;

                        if (collision.prev == null)
                            collision = collision.targ;
                        else
                            collision = collision.prev;
                    }
                }
            }
        }
 

        public float GetPowerLength()
        {
            float maxPathLength = 0.0f;

            foreach (var path in pathCollisions)
            {
                float pathLength = 0.0f;

                foreach (var collision in path.Value)
                {
                    var prev = collision.prev;

                    if (prev == null)
                        continue;
                    else if (collision.elementType == LGSCollisionData.ElementType.C)
                        continue;
                    else if (collision.elementType == LGSCollisionData.ElementType.C)
                        break;

                    pathLength += (prev.position - collision.position).magnitude;
                    /*
                    if (prev.elementType == LGSCollisionData.ElementType.BRICK)
                    {
                        pathLength /= Mathf.Abs(Mathf.Cos(prev.targ.ca * Mathf.Deg2Rad));
                    }
                    else if (prev.elementType == LGSCollisionData.ElementType.BRICK)
                    {
                        pathLength /= Mathf.Abs(Mathf.Sin(prev.ca * Mathf.Deg2Rad));
                    }*/

                }

                if (pathLength > maxPathLength)
                    maxPathLength = pathLength;
            }
            //都是0
            return maxPathLength;
        }

        public float RBMScore()
        {
            float score = 0.0f;
            int dirChange = 0;

            difficulty = 0.0f;
            length = 0.0f;

            foreach (var collision in collisions)
            {
                collision.Calculate();

                difficulty += collision.difficulty * 10;
                length += collision.length;

                if (collision.list_prev != null && collision.dir != collision.list_prev.dir) {
                    dirChange += 5;
                }
            }

            score = difficulty + dirChange;

            //這是MCTS中的可靠率分數
            return score;
        }
    }
}

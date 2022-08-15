using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Pathfinder
{
    class LRTAStar
    {
        readonly double[,] Graph;
        readonly int gridSize;   
        Coord2 startNode;
        Coord2 endNode;
        public Dictionary<int, Node> Nodes; //Arbitrary key value for the vertex of the node, followed by the relevant node class.
        int currentNodeVertex;
        Coord2 currentNode;
        int endNodeVertex;
        public int currentTrial = 0;

        public List<Coord2> Path2Grid;

        public LRTAStar(double[,] Graph, Coord2 startNode, Coord2 endNode, int gridSize)
        {
            this.Graph = Graph;
            this.gridSize = gridSize;
            this.startNode = startNode;
            this.endNode = endNode;
            Nodes = new Dictionary<int, Node>();
            CreateNodes();
        }

        //Creates a nodes list with all of the nodes inside it
        private void CreateNodes()
        {
            Node node;
            for (int i = 0; i < Graph.GetLength(0); i++)
            {
                node = new Node();
                node.stateCost = 0;
                node.vertex = i;
                node.gridPosition = VertexToCoord(i);
                Nodes.Add(i, node);
            }

            currentNodeVertex = CoordToVertex(startNode);
            endNodeVertex = CoordToVertex(endNode);
        }

        //This method picks the next 
        private Coord2 NextPosition(int vertexIndex)
        {
            double min = int.MaxValue;
            List<int> minVertex = new List<int>();
            Coord2 nextNodePos;
            int nextVertex = 0;
            //Finding all locations on the grid nearby that can be moved to and calculating the cost of them
            for (int i = 0; i < 8; i++)
            {
                try
                {
                    nextNodePos = Nodes[vertexIndex].gridPosition;
                    //This switch statement chooses adjacent nodes from one of the 8 surrounding nodes
                    switch (i)
                    {
                        case 0:
                            nextNodePos.X += 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 1:
                            nextNodePos.X -= 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 2:
                            nextNodePos.Y += 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 3:
                            nextNodePos.Y -= 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 4:
                            nextNodePos.X += 1;
                            nextNodePos.Y += 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 5:
                            nextNodePos.X += 1;
                            nextNodePos.Y -= 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 6:
                            nextNodePos.X -= 1;
                            nextNodePos.Y += 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                        case 7:
                            nextNodePos.X -= 1;
                            nextNodePos.Y -= 1;
                            nextVertex = CoordToVertex(nextNodePos);
                            break;
                    }
                    nextNodePos = Nodes[nextVertex].gridPosition;
                }
                catch { continue; }
                if (Graph[vertexIndex, nextVertex] > 0)
                {
                    if (min > Graph[vertexIndex, nextVertex] + Nodes[nextVertex].stateCost)
                    {
                        minVertex.Clear();
                        minVertex.Add(nextVertex);
                        min = Graph[vertexIndex, nextVertex] + Nodes[nextVertex].stateCost;
                    }
                    else if (min == Graph[vertexIndex, nextVertex] + Nodes[nextVertex].stateCost)
                    {
                        minVertex.Add(nextVertex);
                    }
                }
            }
            int nextPos;
            //Checking if there's multiple valid moves around and picking one at random
            if (minVertex.Count > 1)
            {
                Random rand = new Random();
                int i = rand.Next(minVertex.Count);
                nextPos = minVertex[i];
            }
            //Otherwise just moving to the next available space
            else
            {
                nextPos = minVertex[0];
            }
            currentNodeVertex = Nodes[nextPos].vertex;
            UpdateCost(vertexIndex, nextPos);
            return Nodes[nextPos].gridPosition;
        }

        //This method updates the cost of a node on the grid to reflect the LRTA* algorithm's cost calculation
        private void UpdateCost(int vertexIndex, int nextVertex)
        {
            Node node, nextNodePos;
            Nodes.TryGetValue(vertexIndex, out node);
            Nodes.TryGetValue(nextVertex, out nextNodePos);
            node.stateCost = Graph[vertexIndex, nextVertex] + nextNodePos.stateCost;

            Nodes[vertexIndex] = node;
        }

        private Coord2 VertexToCoord(int vertex)
        {
            Coord2 c;
            c.X = (vertex % gridSize);
            c.Y = (vertex / gridSize);
            return c;
        }

        public int CoordToVertex(Coord2 c)
        {
            int v = c.Y * gridSize + c.X;
            return v;
        }

        //The build method for the Algorithm
        public void Build()
        {
            Node node;
            //First we look to see if there's already a file containing a graph
            //If one exists we add all the node values from it into our nodes list
            if (File.Exists("trials.txt")) {
                using (StreamReader sr = new StreamReader(("trials.txt")))
                {
                    currentTrial = Convert.ToInt32(sr.ReadLine());
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        string[] l = line.Split(',');
                        node = new Node();
                        node.vertex = Convert.ToInt32(l[0]);
                        node.stateCost = Convert.ToDouble(l[1]);
                        node.gridPosition = VertexToCoord(node.vertex);
                        Nodes[node.vertex] = node;
                    }
                }
            }
            currentTrial++;
            Path2Grid = new List<Coord2>();
            //while (currentTrial <= Graph.GetLength(0))
            //{
            //    while (currentNodeVertex != endNodeVertex)
            //    {
            //        currentNode = NextPosition(currentNodeVertex);
            //    }
            //    currentNodeVertex = CoordToVertex(startNode);
            //    currentTrial++;
            //}
            while (currentNodeVertex != endNodeVertex)
            {
                currentNode = NextPosition(currentNodeVertex);
                Path2Grid.Add(currentNode);
            }
            if (currentNodeVertex == endNodeVertex)
            {
                if (File.Exists("trials.txt"))
                {
                    File.Delete("trials.txt");
                }
                using (StreamWriter sw = File.CreateText("trials.txt"))
                {
                    sw.WriteLine(currentTrial);
                    for (int i = 0; i < Nodes.Count; i++)
                    {
                        Node currentNode = Nodes[i];
                        string line = (currentNode.vertex.ToString() + ',' + currentNode.stateCost.ToString());
                        sw.WriteLine(line);
                    }
                    sw.Close();
                }
            }
        }
    }
}


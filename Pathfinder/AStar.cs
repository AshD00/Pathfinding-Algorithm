using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Pathfinder
{
    class AStar
    {
        Level gameLevel;

        Dictionary<Coord2, double> G; //The movement cost from the starting point to a given space on the grid
        Dictionary<Coord2, double> H; //The estimated movement cost from a given point on the grid to the destination
        Dictionary<int, Coord2> Nodes; //Arbitrary key value, position on grid
        Dictionary<int, Coord2> openNodes; //Arbitrary key value, position on grid
        Dictionary<int, Coord2> closedNodes; //Arbitrary key value, position on grid
        public List<Coord2> Path2Grid;

        public AStar(Level level)
        {
            Nodes = new Dictionary<int, Coord2>();
            openNodes = new Dictionary<int, Coord2>();
            closedNodes = new Dictionary<int, Coord2>();
            G = new Dictionary<Coord2, double>(); //TL;DR Distance travelled
            H = new Dictionary<Coord2, double>(); //TL;DR Distance remaining
            gameLevel = level;
        }

        //Setting the heuristic values for each space on the grid
        private void setH(Coord2 node)
        {
            int i = 0;
            for (int y = 0; y < gameLevel.GridSize; y++)
            {
                for (int x = 0; x < gameLevel.GridSize; x++)
                {
                    CalculateH(i, x, y, node);
                    i++;
                }
            }
        }

        //The method for building a path using the AStar algorithm
        public void Build(double[,] graph, AiBotBase bot, Player plr)
        {
            int[] parents = new int[graph.GetLength(0)];

            int playerPos = plr.GridPosition.Y * gameLevel.GridSize + plr.GridPosition.X;
            int botPos = bot.GridPosition.Y * gameLevel.GridSize + bot.GridPosition.X;

            openNodes.Add(bot.GridPosition.Y * gameLevel.GridSize + bot.GridPosition.X, bot.GridPosition);

            setH(plr.GridPosition);
            G[bot.GridPosition] = 0;
            parents[botPos] = -1;

            do
            {
                double minDistance = int.MaxValue;
                int minVertex = -1;

                foreach (int i in openNodes.Keys)
                {
                    if (G[Nodes[i]] + H[Nodes[i]] < minDistance)
                    {
                        if (gameLevel.ValidPosition(Nodes[i]))
                        {
                            minDistance = G[Nodes[i]] + H[Nodes[i]];
                            minVertex = i;
                        }
                    }
                }
                openNodes.Remove(minVertex);

                for (int i = 0; i < graph.GetLength(0); i++)
                {

                    if (graph[minVertex, i] > 0)
                    {
                        if (closedNodes.ContainsKey(i)) continue;
                        if (!G.ContainsKey(Nodes[i]) || G[Nodes[minVertex]] + graph[minVertex, i] < G[Nodes[i]])
                        {
                            G[Nodes[i]] = G[Nodes[minVertex]] + graph[minVertex, i];
                            parents[i] = minVertex;
                            if (!openNodes.ContainsKey(i)) openNodes.Add(i, Nodes[i]);
                        }
                    }
                }

                closedNodes.Add(minVertex, Nodes[minVertex]);
                if (minVertex == playerPos) break;

                if (minVertex == playerPos)
                {
                    break;
                }
            } while (openNodes.Count > 0);

            Coord2 prev = plr.GridPosition;
            int prevIndex = playerPos;

            Path2Grid = new List<Coord2>();

            do
            {
                Path2Grid.Add(prev);
                prev = Nodes[parents[prevIndex]];
                prevIndex = parents[prevIndex];
            } while (parents[prevIndex] != -1);
        }

        //Calculating the Heuristic Distance of points on the grid relative to the destination
        //This method also adds all Nodes to a dictionary of nodes.
        private void CalculateH(int vertexValue, int xAxis, int yAxis, Coord2 endNode)
        {
            Coord2 start;

            start.Y = yAxis;
            start.X = xAxis;

            H.Add(start, Math.Sqrt(Math.Pow((endNode.Y - start.Y), 2) + Math.Pow((endNode.X - start.X), 2)));
            Nodes.Add(vertexValue, start);
        }
    }
}
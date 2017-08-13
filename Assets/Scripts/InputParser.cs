using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System;
using UnityEngine;

public static class InputParser {

#pragma warning disable CS0168 // Variable is declared but never used
    public static ParserNode[] Parse(string input, out float S2I, out float I2R, out float S2R, out int packetSize) {
        string[] lines = input.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);

        // Read node attributes
        int count = -1;
        try {
            count = int.Parse(lines[0]);
        } catch (ArgumentNullException e) {
            throw new FormatException("Line 1 is empty");
        } catch (FormatException e) {
            throw new FormatException("Line 1 is not a number");
        } catch (OverflowException e) {
            throw new FormatException("Line 1 number is too big");
        }

        ParserNode[] nodes = new ParserNode[count];
        Dictionary<string, int> indices = new Dictionary<string, int>();
        string[] data;

        for (int i = 0; i < count; i++) {
            try {
                data = lines[i + 1].Split(' ');
            } catch (IndexOutOfRangeException e) {
                throw new FormatException("Too few lines. Need node description line at line " + (i + 1));
            }

            // We already have one with this name
            if (indices.ContainsKey(data[0])) continue;

            try { 
                nodes[i] = new ParserNode(data[0], int.Parse(data[1]), int.Parse(data[2]));
            } catch (ArgumentNullException e) {
                throw new FormatException("Line " + (i + 2) + " has less numbers than needed or is empty");
            } catch (FormatException e) {
                throw new FormatException("Line " + (i + 2) + " has text where a number should be");
            } catch (OverflowException e) {
                throw new FormatException("Line " + (i + 2) + " has a too big number");
            }

            indices.Add(nodes[i].name, i);
        }

        // Read connections
        int connectionCount = int.Parse(lines[count + 1]);
        for (int i = count + 2; i < count + connectionCount + 2; i++) {
            try {
                data = lines[i].Split(' ');
            } catch (IndexOutOfRangeException e) {
                throw new FormatException("Too few lines. Need node connection line at line " + i);
            }

            int firstIndex = -1, secondIndex = -1;
            try {
                if (indices.ContainsKey(data[0])) {
                    firstIndex = indices[data[0]];
                } else {
                    throw new FormatException("Line " + (i + 1) + " has a reference to a non-existent node (" + data[0] + ")");
                }

                if (indices.ContainsKey(data[1])) {
                    firstIndex = indices[data[1]];
                } else {
                    throw new FormatException("Line " + (i + 1) + " has a reference to a non-existent node (" + data[1] + ")");
                }
            } catch (ArgumentNullException e) {
                throw new FormatException("Line " + (i + 1) + " has less data than needed");
            }

            int capacity = -1;
            try {
                capacity = int.Parse(data[2]);
            } catch (ArgumentNullException e) {
                throw new FormatException("Line " + (i + 1) + " has no capacity defined");
            } catch (FormatException e) {
                throw new FormatException("Line " + (i + 1) + " has a non-number capacity");
            } catch (OverflowException e) {
                throw new FormatException("Line " + (i + 1) + " has a too big number as capacity");
            }
            nodes[firstIndex].AddConnectedNode(nodes[secondIndex], capacity);
            nodes[secondIndex].AddConnectedNode(nodes[firstIndex], capacity);
        }

        // assign the static data
        try {
            data = lines[lines.Length - 2].Split(' ');
        } catch (IndexOutOfRangeException e) {
            throw new FormatException("Too few lines. No virus attribute line (one before last one)");
        }
        try {
            S2I = float.Parse(data[0].Replace('.', ','));
            I2R = float.Parse(data[1].Replace('.', ','));
            S2R = float.Parse(data[2].Replace('.', ','));

            packetSize = int.Parse(lines[lines.Length - 1]);
        } catch (ArgumentNullException e) {
            throw new FormatException("Last line has less numbers than needed");
        } catch (FormatException e) {
            throw new FormatException("Last line has a non-number");
        } catch (OverflowException e) {
            throw new FormatException("Last line has a too big number");
        } catch (IndexOutOfRangeException e) {
            throw new FormatException("Too few lines. No packet size line (last one)");
        }

        return nodes;
    }
#pragma warning restore CS0168 // Variable is declared but never used

        public static ParserNode[] Random(int nodeCount, out float S2I, out float I2R, out float S2R, out int packetSize) {
        // init variables
        S2I = UnityEngine.Random.Range(0.01f, 0.09f);
        I2R = UnityEngine.Random.Range(0.001f, 0.01f);
        S2R = UnityEngine.Random.Range(0.005f, 0.015f);
        packetSize = (int) Mathf.Pow(2, UnityEngine.Random.Range(6, 9));

        // init nodes
        ParserNode[] nodes = new ParserNode[nodeCount];
        for (int i = 0; i < nodes.Length; i++) {
            nodes[i] = new ParserNode(
                RandomWords.getRandomWords().Substring(0, 3) + RandomWords.getRandomWords().Substring(0, 3),
                (int) (1000 * UnityEngine.Random.Range(1f, 3f)),
                // about 60% should not be infected
                UnityEngine.Random.value <= 0.6f ? 0 : (int) (100 * UnityEngine.Random.Range(0.1f, 1.8f))
           );
        }

        // init connections

        // first connect them in a random order, so shuffle the array and connect them
        nodes = nodes.OrderBy(node => UnityEngine.Random.Range(0f, 10f)).ToArray();

        for (int i = 0; i < nodes.Length - 1; i++) {
            int capacity = RandomCapacity(packetSize);
            nodes[i].AddConnectedNode(nodes[i + 1], capacity);
            nodes[i + 1].AddConnectedNode(nodes[i], capacity);
        }

        // the add random connections
        for (int i = 0; i < nodes.Length - 1; i++) {
            // we do this so we don't get negative amount
            int connectionCount = Mathf.Max(
                // we do this so we don't choose more from the en than we can
                Mathf.Min(UnityEngine.Random.Range(2, 4), nodes.Length - i - 1) - nodes[i].ConnectedTo.Count,
                0
            );

            // we need all the remaining ones because that's all that's left -> don't randomize it
            if (i + connectionCount == nodes.Length - 1) {
                for (int c = i + 1; c < nodes.Length; c++) {
                    int capacity = RandomCapacity(packetSize);
                    nodes[i].AddConnectedNode(nodes[c], capacity);
                    nodes[c].AddConnectedNode(nodes[i], capacity);
                }
            } else {
                // else choose random connections from the remaining ones
                List<int> been = new List<int>(connectionCount);
                for (int c = 0; c < connectionCount; c++) {
                    // choose index which has not been connected
                    int connectIndex;
                    do {
                        connectIndex = UnityEngine.Random.Range(i + 1, nodes.Length - 1);
                    } while (been.Contains(connectIndex));
                    been.Add(connectIndex);

                    // connect them
                    int capacity = RandomCapacity(packetSize);
                    nodes[i].AddConnectedNode(nodes[connectIndex], capacity);
                    nodes[connectIndex].AddConnectedNode(nodes[i], capacity);
                }
            }
        }

        return nodes;
    }

    private static int RandomCapacity(int packetSize) => 
        UnityEngine.Random.Range(1, packetSize) + packetSize* UnityEngine.Random.Range(5, 8);

}

public struct ParserNode {
    public string name;

    public int hostCount;
    public int infectedCount;

    private List<ParserNodeConnection> connectedTo;
    public ReadOnlyCollection<ParserNodeConnection> ConnectedTo {
        get { return connectedTo.AsReadOnly(); }
    }

    public ParserNode(string name, int hostCount, int infectedCount) {
        this.name = name;
        this.hostCount = hostCount;
        this.infectedCount = infectedCount;

        connectedTo = new List<ParserNodeConnection>();
    }

    public void AddConnectedNode(ParserNode node, int capacity) {
        if (!IsConnectedTo(node))
            connectedTo.Add(new ParserNodeConnection(node, capacity));
    }
    public bool IsConnectedTo(ParserNode other) {
        for (int i = 0; i < connectedTo.Count; i++)
            if (connectedTo[i].connectedTo.name == other.name)
                return true;

        return false;
    }

    public struct ParserNodeConnection {
        public ParserNode connectedTo;
        public int capacity;

        public ParserNodeConnection(ParserNode node, int capacity) {
            this.connectedTo = node;
            this.capacity = capacity;
        }
    }
}
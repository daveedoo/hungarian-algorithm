using QuikGraph;

namespace Hungarian
{
    // Decorator of IUndirectedGraph
    public class VertexLabelledGraph : IUndirectedGraph<int, TaggedUndirectedEdge<int, decimal>>
    {
        private readonly IUndirectedGraph<int, TaggedUndirectedEdge<int, decimal>> _graph;
        private readonly decimal[] _labelling;

        /// <summary>
        /// Wraps the <paramref name="graph"/>, assigning 0 to every vertex label.
        /// </summary>
        /// <param name="graph"></param>
        public VertexLabelledGraph(IUndirectedGraph<int, TaggedUndirectedEdge<int, decimal>> graph)
        {
            _graph = graph;
            _labelling = new decimal[graph.VertexCount];
        }

        public decimal GetVertexLabel(int vertex)
        {
            return _labelling[vertex];
        }

        public void SetVertexLabel(int vertex, decimal labelValue)
        {
            _labelling[vertex] = labelValue;
        }

        public void AddValueToVertexLabel(int vertex, decimal valueToAdd)
        {
            _labelling[vertex] += valueToAdd;
        }

        #region Decorator members
        public EdgeEqualityComparer<int> EdgeEqualityComparer => _graph.EdgeEqualityComparer;
        public bool IsDirected => _graph.IsDirected;
        public bool AllowParallelEdges => _graph.AllowParallelEdges;
        public bool IsEdgesEmpty => _graph.IsEdgesEmpty;
        public int EdgeCount => _graph.EdgeCount;
        public IEnumerable<TaggedUndirectedEdge<int, decimal>> Edges => _graph.Edges;
        public bool IsVerticesEmpty => _graph.IsVerticesEmpty;
        public int VertexCount => _graph.VertexCount;
        public IEnumerable<int> Vertices => _graph.Vertices;

        public IEnumerable<TaggedUndirectedEdge<int, decimal>> AdjacentEdges(int vertex) => _graph.AdjacentEdges(vertex);        
        public int AdjacentDegree(int vertex) => _graph.AdjacentDegree(vertex);
        public bool IsAdjacentEdgesEmpty(int vertex) => _graph.IsAdjacentEdgesEmpty(vertex);
        public TaggedUndirectedEdge<int, decimal> AdjacentEdge(int vertex, int index) => _graph.AdjacentEdge(vertex, index);
        public bool TryGetEdge(int source, int target, out TaggedUndirectedEdge<int, decimal> edge) => _graph.TryGetEdge(source, target, out edge);
        public bool ContainsEdge(int source, int target) => _graph.ContainsEdge(source, target);
        public bool ContainsEdge(TaggedUndirectedEdge<int, decimal> edge) => _graph.ContainsEdge(edge);
        public bool ContainsVertex(int vertex) => _graph.ContainsVertex(vertex);
        #endregion // Decorator members
    }
}

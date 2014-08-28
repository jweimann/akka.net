using RTS.Core.Structs;

namespace RTS.ActorRequests
{
    public struct GetPathRequest
    {
        public readonly Vector3 StartPosition;
        public readonly Vector3 Destination;
        public GetPathRequest(Vector3 startPosition, Vector3 destination)
        {
            this.StartPosition = startPosition;
            this.Destination = destination;
        }
        
    }
}

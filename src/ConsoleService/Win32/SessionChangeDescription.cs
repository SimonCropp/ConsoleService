namespace ConsoleService
{
    public readonly struct SessionChangeDescription
    {
        private readonly int _id;

        internal SessionChangeDescription(SessionChangeReason reason, int id)
        {
            Reason = reason;
            _id = id;
        }

        public SessionChangeReason Reason { get; }

        public override bool Equals(object obj)
        {
            if (obj == null || !(obj is SessionChangeDescription))
            {
                return false;
            }
            else
            {
                return Equals((SessionChangeDescription) obj);
            }
        }

        public override int GetHashCode()
        {
            return (int) Reason ^ _id;
        }

        public bool Equals(SessionChangeDescription changeDescription)
        {
            return (Reason == changeDescription.Reason) && (_id == changeDescription._id);
        }

        public static bool operator ==(SessionChangeDescription a, SessionChangeDescription b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(SessionChangeDescription a, SessionChangeDescription b)
        {
            return !a.Equals(b);
        }
    }
}
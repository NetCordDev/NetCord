using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetCord;

public readonly struct GuildUserRoles : IEnumerable<Role>
{
    private readonly IReadOnlyList<ulong> _roleIds;
    private readonly IReadOnlyDictionary<ulong, Role> _roles;

    internal GuildUserRoles(IReadOnlyList<ulong> roleIds, IReadOnlyDictionary<ulong, Role> roles)
    {
        _roleIds = roleIds;
        _roles = roles;
    }

    public Enumerator GetEnumerator() => new(_roleIds, _roles);

    IEnumerator<Role> IEnumerable<Role>.GetEnumerator() => GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public struct Enumerator : IEnumerator<Role>
    {
        private readonly IReadOnlyList<ulong> _roleIds;
        private readonly IReadOnlyDictionary<ulong, Role> _roles;
        private int _position;

        internal Enumerator(IReadOnlyList<ulong> roleIds, IReadOnlyDictionary<ulong, Role> roles)
        {
            _roleIds = roleIds;
            _roles = roles;
        }

        [AllowNull]
        public Role Current
        {
            readonly get
            {
                if (field is not { } current)
                    ThrowInvalidOperation();

                return current;

                [DoesNotReturn]
                [StackTraceHidden]
                static void ThrowInvalidOperation()
                {
                    throw new InvalidOperationException("Enumerator is positioned before the first element of the collection or after the last element.");
                }
            }
            private set;
        }

        readonly object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            int count = _roleIds.Count;

            while (_position < count)
            {
                var roleId = _roleIds[_position++];

                if (_roles.TryGetValue(roleId, out var role))
                {
                    Current = role;
                    return true;
                }
            }

            Current = null;
            return false;
        }

        void IEnumerator.Reset()
        {
            _position = 0;
            Current = null;
        }

        readonly void IDisposable.Dispose()
        {
        }
    }
}

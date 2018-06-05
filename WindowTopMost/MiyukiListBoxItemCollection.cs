using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowTopMost
{
    [System.ComponentModel.ListBindable(false)]
    public class MiyukiListBoxItemCollection : IList, ICollection, IEnumerable
    {
        private MiyukiListBox m_onwer;

        public MiyukiListBoxItemCollection(MiyukiListBox onwer)
        {
            this.m_onwer = onwer;
        }

        internal MiyukiListBox Owner
        {
            get { return this.m_onwer; }
        }


        public ProcessHnd this[int index] { get => (ProcessHnd)Owner.OldItemSource[index]; set => Owner.OldItemSource[index] = value; }

        public int Count => Owner.OldItemSource.Count;

        public object SyncRoot => Owner.OldItemSource.IsReadOnly;

        public bool IsSynchronized => false;

        public bool IsReadOnly => Owner.OldItemSource.IsReadOnly;

        public bool IsFixedSize => false;

        object IList.this[int index] { get => this[index]; set => this[index] = value as ProcessHnd; }

        public int Add(object value)
        {
            if (!(value is ProcessHnd))
            {
                throw new ArgumentException();
            }
            return Owner.OldItemSource.Add(value);
        }

        public void Clear()
        {
            m_onwer.OldItemSource.Clear();
        }

        public bool Contains(object value)
        {
            return m_onwer.OldItemSource.Contains(value);
        }

        public void CopyTo(object[] array, int index)
        {
            m_onwer.OldItemSource.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return Owner.OldItemSource.GetEnumerator();
        }

        public int IndexOf(object value)
        {
            return Owner.OldItemSource.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            Owner.OldItemSource.Insert(index, value);
        }

        public void Remove(object value)
        {
            Owner.OldItemSource.Remove(value);
        }

        public void RemoveAt(int index)
        {
            Owner.OldItemSource.RemoveAt(index);
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo(array.OfType<object>().ToArray<object>(), index);
        }
    }
}

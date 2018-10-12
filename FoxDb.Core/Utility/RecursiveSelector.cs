using System;
using System.Collections;
using System.Collections.Generic;

namespace FoxDb
{
    public class RecursiveSelector<T> : IEnumerable<T>
    {
        public RecursiveSelector(T element, Func<T, IEnumerable<T>> selector)
            : this(new[] { element }, selector)
        {

        }

        public RecursiveSelector(IEnumerable<T> sequence, Func<T, IEnumerable<T>> selector)
        {
            this.Sequence = sequence;
            this.Selector = selector;
        }

        public IEnumerable<T> Sequence { get; private set; }

        public Func<T, IEnumerable<T>> Selector { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            var stack = new Stack<T>(this.Sequence);
            while (stack.Count > 0)
            {
                var element = stack.Pop();
                yield return element;
                var sequence = this.Selector(element);
                stack.PushRange(sequence);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

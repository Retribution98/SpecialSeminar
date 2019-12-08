using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Exceptions;

namespace Utils.Entities
{
    public class Transposition: IEnumerable<int>
    {
        private List<int> elements;

        public int Count => elements.Count;

        public Transposition(int count)
        {
            elements = new List<int>();
            for (var i = 1; i <= count; i++)
            {
                elements.Add(i);
            }
        }

        public Transposition(IList<int> elements)
        {
            var uniqElemants = elements.Distinct().ToList();
            var demission = elements.Max();
            if (elements.Min() != 1
                || demission != elements.Count
                || uniqElemants.Count != elements.Count)
            {
                throw new ModelValidateException();
            }
            this.elements = elements.ToList();
        }

        public Transposition Add(int newElement)
        {
            var newElems = this.elements;
            newElems.Add(newElement);
            return new Transposition(newElems);
        }

        public IEnumerator<int> GetEnumerator()
        {
            return ((IEnumerable<int>)elements).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<int>)elements).GetEnumerator();
        }

        public override string ToString()
        {
            return string.Join(string.Empty, elements);
        }
    }
}

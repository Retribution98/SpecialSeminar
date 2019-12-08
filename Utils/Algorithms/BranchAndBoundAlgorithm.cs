using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Utils.Entities;

namespace Utils.Algorithms
{
    public class BranchAndBoundAlgorithm<V, T> 
        where V: ITreeVertice<T>
    {
        private  List<ITreeVertice<T>> vertices;
        private readonly Func<List<ITreeVertice<T>>, ITreeVertice<T>> choiceVerticeStratagy;

        public BranchAndBoundAlgorithm(Func<List<ITreeVertice<T>>, ITreeVertice<T>> choiceVerticeStratagy = null)
        {
            vertices = new List<ITreeVertice<T>>();
            this.choiceVerticeStratagy = choiceVerticeStratagy ??
                new Func<List<ITreeVertice<T>>, ITreeVertice<T>>(x => x.OrderByDescending(y => y.HighBound).First());
        }

        public ITreeVertice<T> Execute(V startVertice)
        {
            vertices.Add(startVertice);
            var step = 1;
            while (vertices.Count != 1 || vertices.First().LowBound != vertices.First().HighBound)
            {
                if (!vertices.Any())
                {
                    throw new InvalidOperationException(
                        "List vertices was empty at runtime, so resolve cannot be found. Check the calculation of bounds.");
                }

                var vert = choiceVerticeStratagy(vertices);
                var nextVertices = vert.GetNextVertices();

                vertices.Remove(vert);
                vertices.AddRange(nextVertices);

                //Debug.WriteLine($"-> {vert}");//\t({string.Join(',',vertices)})");

                var minHighDilevery = vertices.OrderBy(x => x.HighBound).First();
                vertices.RemoveAll(x => x.LowBound >= minHighDilevery.HighBound && x != minHighDilevery);
                step++;
            }
            Debug.Write(step + "\t");
            return vertices.First().HighResponse;
        }
    }
}

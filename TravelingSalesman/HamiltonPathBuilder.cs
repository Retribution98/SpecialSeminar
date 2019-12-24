using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TravelingSalesman
{
    public class HamiltonPathBuilder
    {
        private readonly Func<IEnumerable<Cluster>, Point, double> getDistantPointToClusters = 
            (IEnumerable<Cluster> clusters, Point point) =>
            {
                double distant = 0;
                foreach (var clusster in clusters)
                {
                    distant += Point.GetLength(point, clusster.Center);
                }
                return distant;
            };

        public HamiltonPathBuilder(Func<IEnumerable<Cluster>, Point, double> getDistantPointToClusters = null)
        {
            if (getDistantPointToClusters != null)
            {
                this.getDistantPointToClusters = getDistantPointToClusters;
            }
        }

        public async Task<HamiltonPath> GetHamiltonianPath(IEnumerable<Point> points, int countClusster, int countStep)
        {
            if (countClusster < 3)
            {
                throw new ArgumentException("Count cluster mast be > 2");
            }
            if (countStep < 0)
            {
                throw new ArgumentException("count step can not be negative");
            }

            // Останов
            if (points.Count() <= 3)
            {
                return new HamiltonPath(points.ToList());
            }

            double distant, minDistant;
            Point selectedPoint = null;

            // Достигли конца ветвления - строим жадным алгоритмом
            if (countStep < 1 || points.Count() <= countClusster)
            {
                var greedyPath = new List<Point>();
                greedyPath.Add(points.First());
                IEnumerable<Point> freePoints = points.Skip(1);
                while (freePoints.Any())
                {
                    minDistant = double.MaxValue;
                    distant = 0;
                    selectedPoint = null;
                    foreach (var p in freePoints)
                    {
                        distant = Point.GetLength(greedyPath.Last(), p);
                        if (distant < minDistant)
                        {
                            minDistant = distant;
                            selectedPoint = p;
                        }
                    }
                    greedyPath.Add(selectedPoint);
                    freePoints = points.Where(x => !greedyPath.Contains(x));
                };
                return new HamiltonPath(greedyPath);
            }

            // найти две наиболее отдаленные точки
            var clusters = GetTwoMostDistantPoint(points)
                .Select(x => new Cluster(x))
                .ToList();
            while (clusters.Count < countClusster)
            {
                double mostDistant = 0;
                Point nextClusterPoint = null;
                foreach (var point in points.Where(x => !clusters.SelectMany(y => y.Points).Contains(x)))
                {
                    distant = getDistantPointToClusters(clusters, point);

                    if (distant > mostDistant)
                    {
                        mostDistant = distant;
                        nextClusterPoint = point;
                    }
                }
                clusters.Add(new Cluster(nextClusterPoint));
            }

            // распределили точки по кластерам
            var pointsToClusters = points.Where(x => !clusters.SelectMany(c => c.Points).Contains(x));
            foreach (var point in pointsToClusters)
            {
                minDistant = double.MaxValue;
                Cluster selectCluster = null;
                foreach (var cluster in clusters)
                {
                    distant = Point.GetLength(point, cluster.Center);
                    if (distant < minDistant)
                    {
                        minDistant = distant;
                        selectCluster = cluster;
                    }
                }
                selectCluster.Points.Add(point);
            }

            // асинхронно ищем внутренние циклы для кластеров
            var hams = new Dictionary<Cluster, Task<HamiltonPath>>();
            foreach (var cluster in clusters)
            {
                hams.Add(cluster, GetHamiltonianPath(cluster.Points, countClusster, countStep - 1));
            }

            // определяем порядок обхода кластеров
            var path = await GetHamiltonianPath(clusters.Select(x => x.Center), clusters.Count, 0);
            var ordedClusters = path.Points
                .Select(p => clusters.First(c => c.Center == p))
                .ToList();

            // ждем завершения подсчета внутренних путей для кластеров
            Task.WaitAll(hams.Values.ToArray());

            // соединяем кластеры
            // выбираем точки для 1 и 2 клстера и формуируем новый путь
            var firstCluster = ordedClusters.First();
            var secondCluster = ordedClusters[1];
            minDistant = double.MaxValue;
            Point from1Cluster = null, from2Cluster = null;
            var hamiltonPath = new List<Point>();

            foreach (var p1 in firstCluster.Points)
            {
                foreach (var p2 in secondCluster.Points)
                {
                    distant = Point.GetLength(p1, p2);
                    if (distant < minDistant)
                    {
                        minDistant = distant;
                        from1Cluster = p1;
                        from2Cluster = p2;
                    }
                }
            }

            hamiltonPath.AddRange(hams[firstCluster].Result.EndBy(from1Cluster));
            hamiltonPath.AddRange(hams[secondCluster].Result.EndBy(from2Cluster));

            // продолжаем формировать путь для оставшихся кластеров
            selectedPoint = null;
            for (var i = 2; i < ordedClusters.Count; i++)
            {
                var selectedCluster = ordedClusters[i];
                distant = 0;
                minDistant = double.MaxValue;
                foreach (var p in selectedCluster.Points)
                {
                    distant = Point.GetLength(hamiltonPath.Last(), p);
                    if (distant < minDistant)
                    {
                        minDistant = distant;
                        selectedPoint = p;
                    }
                }
                hamiltonPath.AddRange(hams[selectedCluster].Result.EndBy(selectedPoint));
            }

            return new HamiltonPath(hamiltonPath);
        }

        private static IList<Point> GetTwoMostDistantPoint(IEnumerable<Point> points)
        {
            if (points.Count() < 2)
            {
                throw new ArgumentException("count points < 2");
            }
            double maxDistant = 0;
            Point first = null;
            Point second = null;
            double distant;
            foreach (var p1 in points)
            {
                foreach (var p2 in points)
                {
                    distant = Point.GetLength(p1, p2);
                    if (distant > maxDistant)
                    {
                        maxDistant = distant;
                        first = p1;
                        second = p2;
                    }
                }
            }
            return new List<Point> { first, second };
        }

    }
}

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OrderPortfolioFormation
{
    public interface IPortfolio
    {
        IEnumerable<Order> GetOrders(int weight, int? takeOrder = null);
    }

    public abstract class PortfolioBase: IPortfolio
    {
        public IReadOnlyList<Order> ActualOrders { get; }

        public PortfolioBase(IList<Order> actualOrders)
        {
            ActualOrders = actualOrders.ToList();
        }

        public abstract IEnumerable<Order> GetOrders(int weight, int? takeOrder = null);
    }

    public class PortfolioReqursion: PortfolioBase
    {
        public Dictionary<(int weight, int take), IList<Order>> SelectedOrders { get; }

        public PortfolioReqursion(IList<Order> actualOrders)
            :base(actualOrders)
        {
            SelectedOrders = new Dictionary<(int weight, int take), IList<Order>>();
        }

        public override IEnumerable<Order> GetOrders(int weight, int? takeOrder = null)
        {
            var take = takeOrder ?? ActualOrders.Count;

            if (SelectedOrders.ContainsKey((weight, take)))
            {
                return SelectedOrders[(weight, take)];
            }

            if (take == 0)
            {
                return new List<Order>();
            }
            if (take == 1)
            {
                return ActualOrders.First().Weight < weight
                    ? new List<Order> { ActualOrders.First() }
                    : new List<Order>();
            }

            if (ActualOrders[take - 1].Weight > weight)
            {
                return GetOrders(weight, take - 1);
            }

            var s1 = GetOrders(weight, take - 1);
            var s2 = GetOrders(weight - ActualOrders[take - 1].Weight, take).ToList();
            s2.Add(ActualOrders[take - 1]);

            var response = s1.Sum(x => x.Price) > s2.Sum(x => x.Price) ? s1 : s2;
            SelectedOrders.Add((weight, take), response.ToList());
            return response;
        }
    }

    public class PortfolioTable: PortfolioBase
    {
        public PortfolioTable(IList<Order> actualOrders)
            : base(actualOrders)
        {

        }

        public override IEnumerable<Order> GetOrders(int weight, int? takeOrder = null)
        {
            var take = takeOrder ?? ActualOrders.Count;
            var firstColumn = new IEnumerable<Order>[weight + 1];
            for (var i = 0; i <= weight; i++)
            {
                firstColumn[i] = ActualOrders.First().Weight < i
                    ? new List<Order> { ActualOrders.First() }
                    : new List<Order>();
            }
            var secondColumn = new IEnumerable<Order>[weight + 1];

            for (var i = 1; i < take; i++)
            {
                for (var j = 0; j <= weight; j++)
                {
                    secondColumn[j] = ActualOrders[i].Weight > j
                        ? firstColumn[j]
                        : firstColumn[j].Sum(x => x.Price) > firstColumn[j - ActualOrders[i].Weight].Sum(x => x.Price) + ActualOrders[i].Price
                            ? firstColumn[j]
                            : firstColumn[j - ActualOrders[i].Weight].Concat(new List<Order> { ActualOrders[i] });
                }
                firstColumn = secondColumn;
                secondColumn = new IEnumerable<Order>[weight + 1];
            }
            return firstColumn[weight];
        }
    }

    public class PortfolioFast : PortfolioReqursion
    {
        public enum FastPercent
        {
            High = 10,
            Middle = 30,
            Low = 50
        }

        public PortfolioFast(IList<Order> actualOrders)
            //: base(actualOrders.OrderByDescending(x => x.Price / x.Weight).ToList())
            : base(actualOrders.OrderBy(x => x.Weight).ToList())
        {

        }

        public IEnumerable<Order> GetOrdersFast(int weight, FastPercent percent)
        {
            return GetOrders(weight, this.ActualOrders.Count * (int)percent / 100);
        }
    }
}

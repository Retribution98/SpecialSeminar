using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utils.Entities;

namespace GoodsDelivery
{
    public class Delivery
    {
        public int Id { get; protected set; }

        public int DirectiveTime { get; protected set; }

        public IReadOnlyDictionary<Delivery, int> TimeToOther { get; protected set; }

        private Delivery()
        {

        }

        public static IList<Delivery> Build(IList<int> directiveTimes, IList<IList<int>> timesToOther)
        {
            var delivers = new List<Delivery>();
            for (var i = 0; i <= directiveTimes.Count; i++)
            {
                delivers.Add(new Delivery()
                {
                    Id = i,
                    DirectiveTime = i == 0 ? default(int) : directiveTimes[i - 1]
                });
            }
            foreach(var del in delivers)
            {
                var dict = new Dictionary<Delivery, int>(delivers.Count);
                for (var i=0; i < delivers.Count; i++)
                {
                    var toDeliver = delivers.Find(x => x.Id == i);
                    dict.Add(toDeliver, timesToOther[del.Id][i]);
                }
                del.TimeToOther = dict;
            }

            return delivers;
        }
    }

    public class DeiveryVerticeBase : ITreeVertice<Delivery>
    {
        public IReadOnlyList<Delivery> SelectedVertices { get; }

        public IReadOnlyList<Delivery> AvailableVertices { get; }

        protected int SpentTime { get; }

        public int HighBound { get; }

        public int LowBound { get; }

        public ITreeVertice<Delivery> HighResponse { get; protected set; }

        public IEnumerable<ITreeVertice<Delivery>> GetNextVertices()
        {
            foreach(var delivery in AvailableVertices)
            {
                var newSelected = SelectedVertices.ToList();
                newSelected.Add(delivery);
                var newAvailable = AvailableVertices.Where(x => x.Id != delivery.Id).ToList();

                yield return new DeiveryVerticeBase(newSelected, newAvailable);
            }
        }

        public DeiveryVerticeBase(IList<Delivery> selectedDelivers, IList<Delivery> availableDelivers)
        {
            SelectedVertices = selectedDelivers.ToList();
            AvailableVertices = availableDelivers.ToList();
            var time = 0;
            for (var i = 0; i < selectedDelivers.Count - 1; i++)
            {
                time += selectedDelivers[i].TimeToOther[selectedDelivers[i + 1]];
            }
            SpentTime = time;
            HighBound = SetHighBound();
            LowBound = GetLowBound();
        }

        protected virtual int SetHighBound()
        {
            var bound = 0;
            var time = 0;
            var selectedDeliveries = SelectedVertices.ToList();
            var notSelectedDeliveries = AvailableVertices.ToList();
            for (var i = 0; i < selectedDeliveries.Count - 1; i++)
            {
                time += selectedDeliveries[i].TimeToOther[selectedDeliveries[i + 1]];
                if (selectedDeliveries[i + 1].DirectiveTime < time)
                {
                    bound++;
                }
            }
            bound += notSelectedDeliveries.RemoveAll(x => x.DirectiveTime <= time + selectedDeliveries.Last().TimeToOther[x]);
            while(notSelectedDeliveries.Any())
            {
                var delivery = notSelectedDeliveries.OrderBy(x => selectedDeliveries.Last().TimeToOther[x]).First();
                time += selectedDeliveries.Last().TimeToOther[delivery];
                selectedDeliveries.Add(delivery);
                notSelectedDeliveries.Remove(delivery);
                bound += notSelectedDeliveries.RemoveAll(x => x.DirectiveTime <= time + selectedDeliveries.Last().TimeToOther[x]);
            }
            selectedDeliveries.AddRange(AvailableVertices.Where(x => !selectedDeliveries.Contains(x)));
            HighResponse = AvailableVertices.Count == 0 
                ? this 
                : new DeiveryVerticeBase(selectedDeliveries, notSelectedDeliveries);
            return bound;
        }

        protected virtual int GetLowBound()
        {
            var time = 0;
            var bound = 0;
            for (var i = 0; i < SelectedVertices.Count - 1; i++)
            {
                time += SelectedVertices[i].TimeToOther[SelectedVertices[i + 1]];
                if (SelectedVertices[i + 1].DirectiveTime < time)
                {
                    bound++;
                }
            }
            return bound + AvailableVertices.Count(x => x.DirectiveTime <= SpentTime + SelectedVertices.Last().TimeToOther[x]);
        }

        public override string ToString()
        {
            return string.Join("->", SelectedVertices.Select(x => x.Id))+$"({LowBound}/{HighBound})";
        }
    }

    public class DeiveryVerticeMody : DeiveryVerticeBase
    {
        public DeiveryVerticeMody(IList<Delivery> selectedDelivers, IList<Delivery> availableDelivers)
            :base(selectedDelivers, availableDelivers)
        {

        }

        protected override int GetLowBound()
        {
            var time = 0;
            var bound = 0;
            for (var i = 0; i < SelectedVertices.Count - 1; i++)
            {
                time += SelectedVertices[i].TimeToOther[SelectedVertices[i + 1]];
                if (SelectedVertices[i + 1].DirectiveTime < time)
                {
                    bound++;
                }
            }

            return bound + GetSubLowBound(AvailableVertices.ToList(), SelectedVertices.Last(), time, 2);
        }

        private int GetSubLowBound(IList<Delivery> deliveries, Delivery last, int time, int step)
        {
            if (step == 0)
            {
                return deliveries.Count(x => x.DirectiveTime <= time + last.TimeToOther[x]);
            }
            var bounds = new List<int>();
            foreach(var del in deliveries)
            {
                var bound = GetSubLowBound(deliveries.Where(x => x != del).ToList(), del, time + last.TimeToOther[del], step - 1);
                if(del.DirectiveTime < time + last.TimeToOther[del])
                {
                    bound++;
                }
                bounds.Add(bound);
            }
            return bounds.Count != 0 ? bounds.Min() : default(int);
        }
    }
}

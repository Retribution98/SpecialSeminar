using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using Utils.Entities;
using StreamSuppliers;
using Xunit;

namespace StreamSuppliers.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            string zipPath = @"..\..\..\Task4.zip";
            using (var archive = ZipFile.OpenRead(zipPath))
            {
                foreach (var entry in archive.Entries)
                {
                    if (!entry.FullName.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                    using (var reader = new StreamReader(entry.Open()))
                    {
                        var inputDto = GetInputDto(reader);

                        var streamGraph = Program.GetStreamGraph(inputDto);

                    }
                }
            }
        }

        private InputDto GetInputDto(StreamReader reader)
        {
            var inputDto = new InputDto();
            inputDto.CountSupplier = int.Parse(reader.ReadLine());
            inputDto.CountCustomers = int.Parse(reader.ReadLine());
            inputDto.CountTicks = int.Parse(reader.ReadLine());
            reader.ReadLine();
            inputDto.MaxSendBySupplier = reader.ReadLine()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(x => int.Parse(x))
                .ToArray();
            reader.ReadLine();
            inputDto.MaxSendBySupplierInTick = new Dictionary<int, int[]>(inputDto.CountSupplier);
            for (var i = 0; i < inputDto.CountSupplier; i++)
            {
                inputDto.MaxSendBySupplierInTick.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .ToArray());
            }
            reader.ReadLine();
            inputDto.MaxGetByCustomerInTick = new Dictionary<int, int[]>(inputDto.CountCustomers);
            for (var i = 0; i < inputDto.CountCustomers; i++)
            {
                inputDto.MaxGetByCustomerInTick.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x))
                    .ToArray());
            }
            reader.ReadLine();
            inputDto.SupplierIdsForCustomer = new Dictionary<int, int[]>(inputDto.CountCustomers);
            for (var i = 0; i < inputDto.CountCustomers; i++)
            {
                inputDto.SupplierIdsForCustomer.Add(i, reader.ReadLine()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.Parse(x) - 1)
                    .ToArray());
            }

            return inputDto;
        }
    }
}

﻿// See https://aka.ms/new-console-template for more information

using BenchmarkDotNet.Running;
using NexusMods.Hashing.xxHash3.Benchmarks;

BenchmarkRunner.Run<HashRandomDataBench>();

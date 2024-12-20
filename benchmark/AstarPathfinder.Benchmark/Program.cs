// See https://aka.ms/new-console-template for more information

using AstarPathfinder.Benchmark;
using BenchmarkDotNet.Running;

// var switcher = new BenchmarkSwitcher([
//     typeof(Pathfinding)
// ]);
//
// args = new[] { "0" };
// switcher.Run(args);

var summary = BenchmarkRunner.Run<Pathfinding>();
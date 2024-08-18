using System;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.Profiling;

namespace FakeEventBus.Benchmark.Utilities
{
    public abstract class MonoProfiler : MonoBehaviour
    {
        private const int SampleCount = 32;
		
		[SerializeField] public string m_Identifier;
		[SerializeField, Min(1)] public int m_Iterations;
		
		private Rect m_Area;
		private readonly Stopwatch m_Stopwatch = new();
		private readonly RingBuffer<long> m_Samples = new(SampleCount);
		private readonly Lazy<GUIStyle> m_Style = new(() => new GUIStyle("label")
		{
			fontSize = 48,
			alignment = TextAnchor.MiddleCenter
		});

        protected int Iterations => m_Iterations;

		protected abstract int Order { get; }

        protected abstract void OnBeginSample();

		protected abstract void Sample(int i);

		private void Awake()
		{
			var height = (float) Screen.height / 3;
			m_Area = new Rect(0, Order * height, Screen.width, height);
		}

		private void Update()
		{
            OnBeginSample();
			m_Stopwatch.Restart();
			Profiler.BeginSample(m_Identifier);
			for (int i = 0; i < m_Iterations; i++) Sample(i);
			Profiler.EndSample();
			m_Stopwatch.Stop();
			m_Samples.Push(m_Stopwatch.ElapsedMilliseconds);
		}

		private void OnGUI()
		{
			GUI.Label(m_Area, $"{m_Identifier}: {Average(m_Samples)} ms", m_Style.Value);
		}

		private static long Average(RingBuffer<long> buffer)
		{
			long total = 0;

			for (int i = 0; i < buffer.Length; i++)
			{
				total += buffer[i];
			}

			return total / buffer.Length;
		}
    }
}

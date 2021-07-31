using FluentAssertions;
using FluentAssertions.Equivalency;
using System;
using System.Collections.Generic;
using System.Text;

namespace BookOrder.Tests.Common
{
	public static class FluentAssertionsExtensions
	{
		/// <summary>
		/// Returns a bool so it can be used in Mock Verify(It.Is) assertions.
		/// If exceptions is thrown, it won't be caught so the details is preserved.
		/// </summary>
		public static bool IsEquivalentTo<TActual, TExpected>(this TActual actual, TExpected expected, string because = "", params object[] becauseArgs)
		{
			actual.Should().BeEquivalentTo(expected, because, becauseArgs);

			return true;
		}

		/// <summary>
		/// Returns a bool so it can be used in Mock Verify(It.Is) assertions.
		/// If exceptions is thrown, it won't be caught so the details is preserved.
		/// </summary>
		public static bool IsEquivalentTo<TActual, TExpected>(this TActual actual, TExpected expected, Func<EquivalencyAssertionOptions<TExpected>, EquivalencyAssertionOptions<TExpected>> config, string because = "", params object[] becauseArgs)
		{
			actual.Should().BeEquivalentTo(expected, config, because, becauseArgs);

			return true;
		}
	}
}

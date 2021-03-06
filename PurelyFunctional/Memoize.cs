﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PurelyFunctional
{
	public interface IMemoized<T>
	{
		T Value { get; }
	}

	public static class Memoizer
	{
		public static IMemoized<T> Memoize<T>(this Func<T> f)
		{
			return new MemoizedFunc<T>(f);
		}
		public static IMemoized<T> Memoize<T>(this T val)
		{
			return new MemoizedValue<T>(val);
		}
		private sealed class MemoizedValue<T> : IMemoized<T>
		{
			private readonly T value;
			public MemoizedValue(T value)
			{
				this.value = value;
			}
			#region IMemoized<T> Members

			public T Value
			{
				get { return value; }
			}

			#endregion
		}
		private sealed class MemoizedFunc<T> : IMemoized<T>
		{
			private T value;
			private Exception exception;
			private bool hasRun =false;
			private readonly Func<T> func;
			
			public MemoizedFunc(Func<T> func)
			{
				this.func = func;
			}

			#region IMemoized<T> Members

			public T Value
			{
				get 
				{
					if(!hasRun)
					{
						hasRun = true;
						try
						{
							value = func();
						}
						catch(Exception e)
						{
							exception = e;
							throw;
						}
					}
					if(exception != null)
						throw exception;
					return value;
				}
			}

			#endregion
		}
	}
}

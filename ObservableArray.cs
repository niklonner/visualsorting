using System;

public class CompareEvent<T> : EventArgs {
       private T _fst;
       private T _snd;
       
       public T fst { get { return _fst; } }
       public T snd { get { return _snd; } }
}

public class ObservableArray<T> where T : IComparable<T>  {
       private Wrapper[] array;

       public T this[int i] {
              get {
                  return array[i].wrapped;
              }
              set {
                  array[i] = new Wrapper(value);
              }
       }

       public ObservableArray() {
              array = new Wrapper[10];
       }

       public ObservableArray(T[] arr) {
              array = new Wrapper[arr.Length];
              int i=0;
              foreach (T t in arr) {
                    this[i++] = t;
              }
       }
       
       protected class Wrapper : IComparable<Wrapper> {
             public T wrapped;
             public Wrapper (T val) {
                    wrapped = val;
             }

             public int CompareTo(Wrapper other) {
                    return wrapped.CompareTo(other.wrapped);
             }
       }

}

public class MainClass {
       public static void Main(String[] args) {
              ObservableArray<int> arr = new ObservableArray<int>();
       }
}
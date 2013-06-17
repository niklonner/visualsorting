using System;
using System.Collections.Generic;
using System.Linq;

// (Yes, the class can use an underlying list but
// one purpose of this exercise is for me to write
// some C#.)
public class ObservableList<T> : IList<T> {
    private T[] array;
    private int Size;

    public int Count {
        get {
            return Size;
        }
        private set {
            Size = value;
        }
    }
    
    public bool IsReadOnly {
        get {
            return true;
        }
    }

    public T this[int index] {
        get {
            CheckBounds(index,true);
            return array[index];
        }
        set {
            CheckBounds(index,false);
            fireAssign(index,value);
            if (index == Count) {
                EnsureCapacity(index+1);
                Count++;
            }
            array[index] = value;
        }
    }

    public event EventHandler<CompareEvent<T>> Compare;
    public event EventHandler<AssignEvent<T>> Assign;

    public ObservableList() {
        array = new T[10];
    }

    private void CheckBounds(int i, bool tight) {
        if (i < 0 || (i >= Count && tight) || (i > Count && !tight)) {
            throw new ArgumentOutOfRangeException();
        }
    }

    private void EnsureCapacity(int size) {
        if (size > array.Length) {
            Expand();
        }
    }
    
    private void Expand() {
        int OldSize = array.Length;
        T[] NewArr = new T[OldSize*2];
        for (int i=0;i<OldSize;i++) {
            NewArr[i] = array[i];
        }
        array = NewArr;
    }

    private void fireCompare(T e1, T e2) {
        CompareEvent<T> args = new CompareEvent<T>(e1, e2);
        if (Compare != null) {
            Compare(this, args);
        }
    }

    private void fireAssign(int index, T obj) {
        AssignEvent<T> args = new AssignEvent<T>(index, obj);
        if (Assign != null) {
            Assign(this, args);
        }
    }

    public int IndexOf(T elem) {
        Comparer<T> comp = Comparer<T>.Default;
        for(int i=0;i<Count;i++) {
            if (comp.Compare(array[i],elem)==0) {
                return i;
            }
        }
        return -1;
    }
    
    public void Insert(int i, T elem) {
        CheckBounds(i,false);
        if (i==Count) {
            EnsureCapacity(i+1);
            array[i]=elem;
            Count++;
        } else {
            for (int j=Count-1;j>i;j--) {
                array[j] = array[j-1];
            }
            array[i] = elem;
        }        
    }
    
    public void RemoveAt(int i) {
        CheckBounds(i,true);
        for (int j=i;j<Count-1;j++) {
            array[j] = array[j+1];
        }
        Count--;
    }
    
    System.Collections.IEnumerator 
            System.Collections.IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }
    
    public IEnumerator<T> GetEnumerator() {
        return GetEnumeratorHelper().GetEnumerator();
    }
    
    private IEnumerable<T> GetEnumeratorHelper() {
        for (int i=0;i<Count;i++) {
            yield return array[i];
        }
    }
    
    public void Add(T elem) {
        EnsureCapacity(Count+1);
        array[Count++] = elem;
    }
    
    public void Clear() {
        Count = 0;
    }
    
    public bool Contains(T elem) {
        return false;
    }
    
    public void CopyTo(T[] arr, int i) {
        foreach (T t in array) {
            arr[i++] = t;
        }
    }
    
    public bool Remove(T elem) {
        int Index = IndexOf(elem);
        if (Index < 0) {
            return false;
        } else {
            RemoveAt(Index);
            return true;
        }
    }
    
}

public class CompareEvent<T> : EventArgs {
       public CompareEvent(T fst, T snd) {
              this.fst = fst;
              this.snd = snd;
       }
       
       public T fst { get; private set;}
       public T snd { get; private set;}
}

public class AssignEvent<T> : EventArgs {
       public AssignEvent(int index, T obj) {
              this.index = index;
              this.obj = obj;
       }

       public int index { get; private set; }
       public T obj { get; private set; }
}

    internal class Wrapper<T> : IComparable<Wrapper<T>> where T : IComparable<T> {
         public T wrapped;

         public Wrapper (T val) {
                wrapped = val;
         }

         public int CompareTo(Wrapper<T> other) {
//                outer.fireCompare(new CompareEvent<T>(wrapped, other.wrapped));
                return wrapped.CompareTo(other.wrapped);
         }
    }

public static class Sorting {
    public static void insertionSort<T>(IList<T> array) where T : IComparable<T> {
        for (int i=1;i<array.Count;i++) {
            int j = i;
            T val = array[j];
            while (j>0 && val.CompareTo(array[j-1]) < 0) {
                array[j] = array[j-1];
                j--;
            }
            array[j] = val;
        }
    }
}

public class MainClass {
    public static void Main(String[] args) {
        ObservableList<int> list = new ObservableList<int>();
        list.Add(2);
        list.Add(5);
        list.Add(3);
//        list[2]=list[0];
        Console.WriteLine(list);
        Sorting.insertionSort(list);
        Console.WriteLine(list);
//        foreach (int i
//        ObservableArray<int> arr = new ObservableArray<int>(new int[] {3,2,5});
/*             foreach (int x in arr) {
                Console.WriteLine(x);
             }
             Controller<int> c = new Controller<int>(arr, new ASCIIView<int>());
//             Sorting.insertionSort(arr);
             c.sort<int>(Sorting.insertionSort);
             foreach (int x in arr) {
                Console.WriteLine(x);
             }                       */  
    }
}

public interface ArrayView<T> {
    void init();
    EventHandler<CompareEvent<T>> compareHandler {get;}
    EventHandler<AssignEvent<T>> assignHandler {get;}
}

public class Controller<T> where T : IComparable<T> {
//    private ObservableArray<T> arr;
    private ArrayView<T> view;
    
    public Controller(ArrayView<T> view) {
//        this.arr = arr;
        this.view = view;
//        arr.Compare += view.compareHandler;
//        arr.Assign += view.assignHandler;
    }        
    
    public void sort<X>(Action<X[]> func) where X : IComparable<X> {
//        arr.sort(func);
    }
}

public class ASCIIView<T> : ArrayView<T> {
    public void init() {
    
    }
    
    public EventHandler<CompareEvent<T>> compareHandler { get; private set; }
    public EventHandler<AssignEvent<T>> assignHandler { get; private set; }
    
    public ASCIIView() {
        compareHandler = compareEvent;
        assignHandler = assignEvent;
    }
    
    public void compareEvent(Object sender, CompareEvent<T> e) {
        Console.WriteLine("comparing {0} and {1}", e.fst, e.snd);
    }
    
    public void assignEvent(Object sender, AssignEvent<T> e) {
        Console.WriteLine("assigning {0} to index {1}", e.obj, e.index);
    }
}

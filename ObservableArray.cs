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
            if (index == Count) {
                EnsureCapacity(index+1);
                Count++;
            }
            array[index] = value;
            fireAssign(index);
        }
    }

    public event EventHandler<AssignEvent> Assign;

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

    private void fireAssign(int index) {
        AssignEvent args = new AssignEvent(index);
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

    public override String ToString() {
        if (Count == 0) {
            return "[]";
        } else {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("[");
            foreach (T t in array.Take(Count-1)) {
                sb.Append(t);
                sb.Append(" ");
            }
            sb.Append(array[Count-1]);
            sb.Append("]");
            return sb.ToString();
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

public class AssignEvent : EventArgs {
       public AssignEvent(int index) {
              this.index = index;
       }

       public int index { get; private set; }
}

public static class Sorting {
    public static void insertionSort<T>(IList<T> array, IComparer<T> comp) {
//        Comparer<T> comp = Comparer<T>.Default;
        for (int i=1;i<array.Count;i++) {
            int j = i;
            T val = array[j];
            while (j>0 && comp.Compare(val,array[j-1]) < 0) {
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
//        Sorting.insertionSort(list);
        Console.WriteLine(list);
        Controller<int> c = new Controller<int>(list, new ASCIIView<int>());
        c.sort(Sorting.insertionSort);
        
//        foreach (int i
//        ObservableArray<int> arr = new ObservableArray<int>(new int[] {3,2,5});
/*             foreach (int x in arr) {
                Console.WriteLine(x);
             }

             foreach (int x in arr) {
                Console.WriteLine(x);
             }                       */  
    }
}

public interface ArrayView<T> {
    void init();
    EventHandler<CompareEvent<T>> compareHandler {get;}
    EventHandler<AssignEvent> assignHandler {get;}
}

public class Controller<T> where T : IComparable<T> {
    private ObservableList<T>list;
    private ArrayView<T> view;
    private EventComparer comp;
    
    public Controller(IEnumerable<T> list, ArrayView<T> view) {
        this.list = new ObservableList<T>();
        foreach (T t in list) {
            this.list.Add(t);
        }
        this.view = view;
//        arr.Compare += view.compareHandler;
        this.list.Assign += view.assignHandler;
        comp = new EventComparer();
        comp.CompareHandler += view.compareHandler;
    }        
    
    public void sort(Action<IList<T>,IComparer<T>> func) {
//        arr.sort(func);
//        Action<IList<Wrapper>> l1 = null;
//        Action<IList<Object>> l2 = null;
//        l2=l1;
        func(list,comp);
    }

    private class EventComparer : IComparer<T>{

        public event EventHandler<CompareEvent<T>> CompareHandler;

        private void fireCompare(T e1, T e2) {
            CompareEvent<T> args = new CompareEvent<T>(e1, e2);
            if (CompareHandler != null) {
                CompareHandler(this, args);
            }
        }
        
        public int Compare(T t1, T t2) {
            Comparer<T> comp = Comparer<T>.Default;
            fireCompare(t1,t2);
            return comp.Compare(t1,t2);
        }
    }
    
}

public class ASCIIView<T> : ArrayView<T> {
    public void init() {
    
    }
    
    public EventHandler<CompareEvent<T>> compareHandler { get; private set; }
    public EventHandler<AssignEvent> assignHandler { get; private set; }
    
    public ASCIIView() {
        compareHandler = compareEvent;
        assignHandler = assignEvent;
    }
    
    public void compareEvent(Object sender, CompareEvent<T> e) {
        Console.WriteLine("comparing {0} and {1}", e.fst, e.snd);
    }
    
    public void assignEvent(Object sender, AssignEvent e) {
        Console.WriteLine("assigning to index {0}", e.index);
    }
}

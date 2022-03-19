using System;
using System.Collections.Generic;
using System.Text;

namespace Server.Game
{
    public interface IJob
    {
        void Excute();
    }

    public class Job : IJob
    {
        /* 어떤 객체를 가지고, 어떤 기능을 수행한다 => Action, Func으로 핸들링 가능 */
        Action _action;
        public Job(Action action)
        {
            _action = action;
        }

        public void Excute()
        {
            _action.Invoke();
        }
    }

    public class Job<T1> : IJob
    {
        /* 어떤 객체를 가지고, 어떤 기능을 수행한다 => Action, Func으로 핸들링 가능 */
        Action<T1> _action;
        T1 _t1;
        public Job(Action<T1> action, T1 t1)
        {
            _action = action;
            _t1 = t1;
        }

        public void Excute()
        {
            _action.Invoke(_t1);
        }
    }

    public class Job<T1, T2> : IJob
    {
        /* 어떤 객체를 가지고, 어떤 기능을 수행한다 => Action, Func으로 핸들링 가능 */
        Action<T1, T2> _action;
        T1 _t1;
        T2 _t2;
        public Job(Action<T1, T2> action, T1 t1, T2 t2)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
        }

        public void Excute()
        {
            _action.Invoke(_t1, _t2);
        }
    }

    public class Job<T1, T2, T3> : IJob
    {
        /* 어떤 객체를 가지고, 어떤 기능을 수행한다 => Action, Func으로 핸들링 가능 */
        Action<T1, T2, T3> _action;
        T1 _t1;
        T2 _t2;
        T3 _t3;
        public Job(Action<T1, T2, T3> action, T1 t1, T2 t2, T3 t3)
        {
            _action = action;
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;
        }

        public void Excute()
        {
            _action.Invoke(_t1, _t2, _t3);
        }
    }
}

using System.Collections.Generic;
using System.Dynamic;

namespace WBBEntity.Extensions
{
    public class DynamicDictionary : DynamicObject
    {
        Dictionary<string, object> dictionary
            = new Dictionary<string, object>();

        public int Count
        {
            get
            {
                return dictionary.Count;
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string name = binder.Name.ToLower();
            return dictionary.TryGetValue(name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            dictionary[binder.Name.ToLower()] = value;
            return true;
        }
    }

    //public class DynamicDictionary : IDynamicMetaObjectProvider
    //{
    //    IDictionary<string, object> dictionary
    //         = new Dictionary<string, object>();

    //    #region IDynamicMetaObjectProvider implementation
    //    public DynamicMetaObject GetMetaObject(Expression expression)
    //    {
    //        return new DynamicDictionaryMetaObject(expression,
    //            BindingRestrictions.GetInstanceRestriction(expression, this), this);
    //    }
    //    #endregion

    //    #region Helper methods for dynamic meta object support
    //    internal object setValue(string name, object value)
    //    {
    //        dictionary.Add(name, value);
    //        return value;
    //    }

    //    internal object getValue(string name)
    //    {
    //        object value;
    //        if (!dictionary.TryGetValue(name, out value))
    //        {
    //            value = null;
    //        }
    //        return value;
    //    }

    //    internal IEnumerable<string> getDynamicMemberNames()
    //    {
    //        return dictionary.Keys;
    //    }
    //    #endregion


    //}

    //public class DynamicDictionaryMetaObject : DynamicMetaObject
    //{
    //    Type objType;

    //    public DynamicDictionaryMetaObject(Expression expression, BindingRestrictions restrictions, object value)
    //        : base(expression, restrictions, value)
    //    {
    //        objType = value.GetType();
    //    }

    //    public override DynamicMetaObject BindGetMember(GetMemberBinder binder)
    //    {
    //        var self = this.Expression;
    //        var dynObj = (DynamicDictionary)this.Value;
    //        var keyExpr = Expression.Constant(binder.Name);
    //        var getMethod = objType.GetMethod("getValue", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var target = Expression.Call(Expression.Convert(self, objType),
    //                                     getMethod,
    //                                     keyExpr);
    //        return new DynamicMetaObject(target,
    //            BindingRestrictions.GetTypeRestriction(self, objType));
    //    }

    //    public override DynamicMetaObject BindSetMember(SetMemberBinder binder, DynamicMetaObject value)
    //    {
    //        var self = this.Expression;
    //        var keyExpr = Expression.Constant(binder.Name);
    //        var valueExpr = Expression.Convert(value.Expression, typeof(object));
    //        var setMethod = objType.GetMethod("setValue", BindingFlags.NonPublic | BindingFlags.Instance);
    //        var target = Expression.Call(Expression.Convert(self, objType),
    //        setMethod,
    //        keyExpr,
    //        valueExpr);
    //        return new DynamicMetaObject(target,
    //            BindingRestrictions.GetTypeRestriction(self, objType));
    //    }

    //    public override IEnumerable<string> GetDynamicMemberNames()
    //    {
    //        var dynObj = (DynamicDictionary)this.Value;
    //        return dynObj.getDynamicMemberNames();
    //    }
    //}
}

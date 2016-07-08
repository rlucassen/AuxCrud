namespace AuxCrud.ViewModel.Forms
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Helpers;
    using Inputs;
    using Model;
    using NHibernate.Util;
    using ViewModel;

    public class FormComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public FormComponent(ViewModel<TOwner, TViewModel> viewModel)
        {
            ViewModel = viewModel;

            Rows = new List<RowComponent<TOwner, TViewModel>>();
        }

        public IList<RowComponent<TOwner, TViewModel>> Rows { get; set; }
        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }

        public RowComponent<TOwner, TViewModel> Row(string label = null, params int[] rowSplits)
        {
            var rowComponent = new RowComponent<TOwner, TViewModel>(ViewModel, this, label, rowSplits);
            Rows.Add(rowComponent);
            return rowComponent;
        }

        public InputComponent<TOwner, TViewModel> Input(Expression<Func<TViewModel, object>> expression, BaseInput input)
        {
            var row = Row();
            return row.Input(expression, input);
        }
    }

    public class RowComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public RowComponent(ViewModel<TOwner, TViewModel> viewModel, FormComponent<TOwner, TViewModel> form, string label, params int[] rowSplits)
        {
            OwningForm = form;
            ViewModel = viewModel;
            Label = label;
            RowSplits = rowSplits;

            Inputs = new List<InputComponent<TOwner, TViewModel>>();
        }

        private string Label { get; set; }
        private int[] RowSplits { get; set; }
        public IList<InputComponent<TOwner, TViewModel>> Inputs { get; set; }
        public FormComponent<TOwner, TViewModel> OwningForm { get; set; }
        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }

        public InputComponent<TOwner, TViewModel> Input(Expression<Func<TViewModel, object>> expression, BaseInput input)
        {
            var inputComponent = new InputComponent<TOwner, TViewModel>(expression, input, ViewModel, this);
            Inputs.Add(inputComponent);
            return inputComponent;
        }

        public string GetLabel()
        {
            if (Label != null) return Label;

            var firstInput = Inputs.FirstOrDefault();
            var propInfo = LambdaHelper.GetMemberExpression(firstInput.Expression).Member as PropertyInfo;

            var name = LocalizationHelper.String(propInfo);

            return Label ?? name;
        }

        public int[] GetRowSplits()
        {
            if (RowSplits.Length >= Inputs.Count) return RowSplits.Take(Inputs.Count).ToArray();
            var tempSplits = RowSplits.ToList();
            var diff = Inputs.Count - RowSplits.Length;
            var filled = RowSplits.Sum(x => x);

            var fillPer = Convert.ToInt32(Math.Floor((10.0 - filled)/diff));

            var rest = 10 - filled - (fillPer*diff);

            tempSplits.Add(fillPer + rest);
            for (var i = 1; i < diff; i++)
            {
                tempSplits.Add(fillPer);
            }
            return tempSplits.ToArray();
        }
    }

    public class InputComponent<TOwner, TViewModel> where TOwner : ModelBase, new() where TViewModel : ViewModel<TOwner, TViewModel>
    {
        public InputComponent(Expression<Func<TViewModel, object>> expression, BaseInput input, ViewModel<TOwner, TViewModel> viewModel, RowComponent<TOwner, TViewModel> row)
        {
            InputField = input;
            OwningRow = row;
            ViewModel = viewModel;
            Expression = expression;
        }

        public BaseInput InputField { get; set; }
        public RowComponent<TOwner, TViewModel> OwningRow { get; set; }
        public ViewModel<TOwner, TViewModel> ViewModel { get; set; }
        public Expression<Func<TViewModel, object>> Expression { get; set; }

        public RowComponent<TOwner, TViewModel> Row(string label = null, params int[] rowSplits)
        {
            return OwningRow.OwningForm.Row(label, rowSplits);
        }

        public InputComponent<TOwner, TViewModel> Input(Expression<Func<TViewModel, object>> expression, BaseInput input)
        {
            return OwningRow.Input(expression, input);
        }


    }
}
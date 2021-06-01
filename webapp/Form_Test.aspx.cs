using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Code;

public partial class Form_Test : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        GlobalClass gc = new GlobalClass();
        DataTable dt = new DataTable();
        DataTableCollection dtc = null;
        StringBuilder sb = new StringBuilder();

        string FormTemplateID = gc.Req("fid");
        string context = gc.Req("context");


        dtc = gc.GetTables($"exec dcr_sp_form {FormTemplateID}");
        dt = dtc[0];

        IRenderer currentRenderer;

        foreach (DataRow row in dt.Rows)
        {
            currentRenderer = MapTypeToInputRenderer(row["Type"].ToString());
            var dataFields = new Dictionary<string, string>(){
            {"TemplateFieldId", row["Template_Field_ID"].ToString()},
            {"FieldName", row["Field"].ToString()},
            {"FieldDescription", row["Template_Field_Description"].ToString()},
            {"Choices", row["Choices"].ToString()},
            {"Required", row["Required"].ToString()}
        };
        sb.AppendLine(currentRenderer.Draw(dataFields));
        Form.Text = $@"<form id='{context}'>{sb}</div>";
        }
    }

    private IRenderer MapTypeToInputRenderer(string type)
    {
        switch (type)
        {
            case "Text 100 characters":
                return new TextInput();
            case "Text 4000 characters":
                return new TextAreaInput();
            case "Number":
            case "Money":
                return new NumberInput();
            case "Date":
                return new DateInput();
            case "Yes/No":
                return new BooleanInput();
            case "Choice (single)":
                return new SelectInput();
            case "Choice (multiple)":
                return new MultipleSelectInput();
            case "Files":
                return new FileInput();
            default:
                return new TextInput();
        }
    }

    interface IRenderer
    {
        string Draw(Dictionary<string, string> DataFields);
    }

    public class TextInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='text' class='form-control form-input' id='{DataFields["FieldName"]}' />
                    </div>";
        }
    }

    public class NumberInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='number' class='form-control form-input' id='{DataFields["FieldName"]}'/>
                    </div>";
        }
    }

    public class TextAreaInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <textarea class='form-control form-input' id='{DataFields["FieldName"]}' rows='3'></textarea>
                     </div>";
        }
    }

    public class BooleanInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='form-group form-check d-flex flex-row align-items-center form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <input type = 'checkbox' class='form-check-input form-input' id='{DataFields["FieldName"]}'>
                        <label class='form-check-label' for='{DataFields["FieldName"]}'>
                            <span class='d-flex flex-row align-items-baseline bold'>
                                {DataFields["FieldName"]}
                                &nbsp
                                <small class='form-text text-muted'>{DataFields["FieldDescription"]}.</small>
                            </span>
                        </label>
                      </div>";
        }
    }

    public class DateInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='date' class='form-control datepicker form-input' id='{DataFields["FieldName"]}'/>
                    </div>";
        }
    }

    public class SelectInput : IRenderer
    {
        string options { get; set; }
        public string Draw(Dictionary<string, string> DataFields)
        {
            options = buildOptions(DataFields["Choices"]);

            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <select class='form-control form-input' id='exampleFormControlSelect2'>
                            {options}
                        </select>
                     </div>";
        }

        private string buildOptions(string choices)
        {
            List<string> optionsList = new List<string>(
                choices.Split(new string[] { "<br>" }, StringSplitOptions.None));

            StringBuilder options = new StringBuilder();
            foreach (string option in optionsList)
            {
                options.AppendLine($"<option value={option}>{option}</option>");
            }
            return options.ToString();
        }
    }

    public class MultipleSelectInput : IRenderer
    {
        string options { get; set; }
        public string Draw(Dictionary<string, string> DataFields)
        {
            options = buildOptions(DataFields["Choices"]);

            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <div class='multi-select-wrapper'>
                            {options}
                        </div>
                     </div>";
        }

        private string buildOptions(string choices)
        {
            List<string> optionsList = new List<string>(
                choices.Split(new string[] { "<br>" }, StringSplitOptions.None));

            StringBuilder options = new StringBuilder();
            foreach (string option in optionsList)
            {
                string checkbox = $@"<div class='form-check form-check-inline'>
                                        <input class='form-check-input form-input' type='checkbox' id='{option}' value='{option}'/>
                                        <label class='form-check-label' for='{option}'>{option}</label>
                                     </div>";
                options.AppendLine($"{checkbox}");
            }
            return options.ToString();
        }
    }

    public class FileInput : IRenderer
    {
        public string Draw(Dictionary<string, string> DataFields)
        {
            return $@"<div class='mb-3 form-input-wrapper' fr='{DataFields["Required"]}' fid='{DataFields["TemplateFieldId"]}'>
                        <label for='{DataFields["FieldName"]}' class='form-label d-flex flex-row align-items-baseline justify-content-between bold'>
                            {DataFields["FieldName"]}
                            <div class='form-text text-muted'>{DataFields["FieldDescription"]}.</div>
                        </label>
                        <input type='file' class='form-control-file form-input' id='{DataFields["FieldName"]}'/>
                    </div>";
        }
    }
};
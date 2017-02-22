"use strict";

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

var PaginatedTable = React.createClass({
   displayName: "PaginatedTable",

   render: function render() {
      var _this = this;

      return React.createElement(
         "div",
         null,
         React.createElement(DataTable, _extends({}, this.props, {
            onSelect: function (key) {
               return _this.props.onSelect(key);
            } })),
         React.createElement(Pagination, { style: { marginTop: "1em", float: "right" },
            pagination: this.props.pagination,
            select: this.props.page,
            onSelect: function (page) {
               return _this.props.onSelectPage(page);
            } })
      );
   }
});

var DataTable = React.createClass({
   displayName: "DataTable",

   render: function render() {
      var _this2 = this;

      var handleRowSelection = function handleRowSelection(rows) {
         if (rows.length > 0) handleSelect(_this2.props.data[rows[0]][_this2.props.itemKey]);
      };

      var handleSelect = function handleSelect(id) {
         if (id != _this2.props.select) _this2.props.onSelect(id);
      };

      var colWidth = function colWidth(i) {
         return { width: _this2.props.colWidths[i] };
      };

      var columns = function columns(data) {
         return _this2.props.headers.map(function (header, index) {
            return React.createElement(
               TableRowColumn,
               { key: header, style: colWidth(index) },
               data[header]
            );
         });
      };

      var rows = this.props.data.map(function (data, index) {
         return React.createElement(
            TableRow,
            { key: data[_this2.props.itemKey], selected: _this2.props.select == data[_this2.props.itemKey] },
            columns(data)
         );
      });

      var headers = this.props.headers.map(function (header, index) {
         return React.createElement(
            TableHeaderColumn,
            { key: header, style: colWidth(index) },
            header
         );
      });

      return React.createElement(
         Table,
         { selectable: true, onRowSelection: handleRowSelection },
         React.createElement(
            TableHeader,
            { displaySelectAll: false, adjustForCheckbox: false },
            React.createElement(
               TableRow,
               null,
               headers
            )
         ),
         React.createElement(
            TableBody,
            { displayRowCheckbox: false, showRowHover: true },
            rows
         )
      );
   }
});

var Pagination = React.createClass({
   displayName: "Pagination",

   render: function render() {
      var _this3 = this;

      var pageButtons = this.props.pagination.map(function (page) {
         return React.createElement(
            Paper,
            { key: page, style: { display: "inline", padding: ".5em 0" } },
            React.createElement(FlatButton, { style: { minWidth: "1em" },
               label: page,
               disabled: _this3.props.select == page,
               onClick: function () {
                  return _this3.props.onSelect(page);
               } })
         );
      });

      return React.createElement(
         "div",
         { style: this.props.style },
         pageButtons
      );
   }
});


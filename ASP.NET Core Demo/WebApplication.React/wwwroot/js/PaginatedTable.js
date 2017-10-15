"use strict";

var _extends = Object.assign || function (target) { for (var i = 1; i < arguments.length; i++) { var source = arguments[i]; for (var key in source) { if (Object.prototype.hasOwnProperty.call(source, key)) { target[key] = source[key]; } } } return target; };

var _createClass = (function () { function defineProperties(target, props) { for (var i = 0; i < props.length; i++) { var descriptor = props[i]; descriptor.enumerable = descriptor.enumerable || false; descriptor.configurable = true; if ("value" in descriptor) descriptor.writable = true; Object.defineProperty(target, descriptor.key, descriptor); } } return function (Constructor, protoProps, staticProps) { if (protoProps) defineProperties(Constructor.prototype, protoProps); if (staticProps) defineProperties(Constructor, staticProps); return Constructor; }; })();

var _get = function get(_x, _x2, _x3) { var _again = true; _function: while (_again) { var object = _x, property = _x2, receiver = _x3; _again = false; if (object === null) object = Function.prototype; var desc = Object.getOwnPropertyDescriptor(object, property); if (desc === undefined) { var parent = Object.getPrototypeOf(object); if (parent === null) { return undefined; } else { _x = parent; _x2 = property; _x3 = receiver; _again = true; desc = parent = undefined; continue _function; } } else if ("value" in desc) { return desc.value; } else { var getter = desc.get; if (getter === undefined) { return undefined; } return getter.call(receiver); } } };

function _classCallCheck(instance, Constructor) { if (!(instance instanceof Constructor)) { throw new TypeError("Cannot call a class as a function"); } }

function _inherits(subClass, superClass) { if (typeof superClass !== "function" && superClass !== null) { throw new TypeError("Super expression must either be null or a function, not " + typeof superClass); } subClass.prototype = Object.create(superClass && superClass.prototype, { constructor: { value: subClass, enumerable: false, writable: true, configurable: true } }); if (superClass) Object.setPrototypeOf ? Object.setPrototypeOf(subClass, superClass) : subClass.__proto__ = superClass; }

var PaginatedTable = (function (_React$Component) {
   _inherits(PaginatedTable, _React$Component);

   function PaginatedTable(props) {
      _classCallCheck(this, PaginatedTable);

      _get(Object.getPrototypeOf(PaginatedTable.prototype), "constructor", this).call(this, props);
   }

   _createClass(PaginatedTable, [{
      key: "render",
      value: function render() {
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
   }]);

   return PaginatedTable;
})(React.Component);

var DataTable = (function (_React$Component2) {
   _inherits(DataTable, _React$Component2);

   function DataTable(props) {
      _classCallCheck(this, DataTable);

      _get(Object.getPrototypeOf(DataTable.prototype), "constructor", this).call(this, props);
   }

   _createClass(DataTable, [{
      key: "render",
      value: function render() {
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
   }]);

   return DataTable;
})(React.Component);

var Pagination = (function (_React$Component3) {
   _inherits(Pagination, _React$Component3);

   function Pagination(props) {
      _classCallCheck(this, Pagination);

      _get(Object.getPrototypeOf(Pagination.prototype), "constructor", this).call(this, props);
   }

   _createClass(Pagination, [{
      key: "render",
      value: function render() {
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
   }]);

   return Pagination;
})(React.Component);


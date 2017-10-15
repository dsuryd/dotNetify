class PaginatedTable extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      return (
         <div>
            <DataTable {...this.props}
                       onSelect={key => this.props.onSelect(key)} />
            <Pagination style={{marginTop: "1em", float: "right"}}
                        pagination={this.props.pagination}
                        select={this.props.page}
                        onSelect={page => this.props.onSelectPage(page)} />
         </div>
      );
   }
}

class DataTable extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      const handleRowSelection = rows => {
         if (rows.length > 0)
            handleSelect(this.props.data[rows[0]][this.props.itemKey]);
      }

      const handleSelect = id => {
         if (id != this.props.select)
            this.props.onSelect(id);
      }

      const colWidth = i => { return { width: this.props.colWidths[i] }; }

      const columns = data => this.props.headers.map((header, index) =>
         <TableRowColumn key={header} style={colWidth(index)}>{data[header]}</TableRowColumn>
      );

      const rows = this.props.data.map((data, index) =>
         <TableRow key={data[this.props.itemKey]} selected={this.props.select == data[this.props.itemKey]}>
            {columns(data)}
         </TableRow>
      );

      const headers = this.props.headers.map((header, index) =>
         <TableHeaderColumn key={header} style={colWidth(index)}>{header}</TableHeaderColumn>
      );

      return (
         <Table selectable={true} onRowSelection={handleRowSelection}>
            <TableHeader displaySelectAll={false} adjustForCheckbox={false}>
               <TableRow>
                  {headers}
               </TableRow>
            </TableHeader>
            <TableBody displayRowCheckbox={false} showRowHover={true}>
               {rows}
            </TableBody>
         </Table>
      );
   }
}

class Pagination extends React.Component {
   constructor(props) {
      super(props);
   }
   render() {
      const pageButtons = this.props.pagination.map(page =>
         <Paper key={page} style={{display: "inline", padding: ".5em 0"}}>
            <FlatButton style={{minWidth: "1em"}}
                        label={page}
                        disabled={this.props.select == page}
                        onClick={() => this.props.onSelect(page)} />
         </Paper>
      );

      return (
         <div style={this.props.style}>{pageButtons}</div>
      );
   }
}
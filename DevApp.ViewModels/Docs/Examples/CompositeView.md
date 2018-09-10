##### CompositeView.js

```jsx
import React from 'react';
import PropTypes from 'prop-types';
import { Scope } from 'dotnetify/react';
import TextBox from './components/TextBox';
import { CompositeViewCss } from './components/css';

const CompositeView = () => (
  <CompositeViewCss>
    <Scope vm="CompositeViewVM">
      <Scope vm="FilterableMovieTableVM">
        <MovieTable />
      </Scope>
      <aside>
        <MovieDetails />
        <Scope vm="FilterableMovieTableVM">
          <MovieFilter />
        </Scope>
      </aside>
    </Scope>
  </CompositeViewCss>
);
```

##### MovieTable.js
```jsx
class MovieTable extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = this.context.connect('MovieTableVM', this) || { Headers: [], Data: [], Pagination: [] };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    return (
      <div>
        <table>
          <tbody>
            <tr>{this.state.Headers.map((text, idx) => <th key={idx}>{text}</th>)}</tr>
            {this.state.Data.map((data, idx) => (
              <tr
                key={idx}
                className={this.state.SelectedKey === data.Rank ? 'selected' : ''}
                onClick={_ => this.dispatchState({ SelectedKey: data.Rank })}
              >
                <td>{data.Rank}</td>
                <td>{data.Movie}</td>
                <td>{data.Year}</td>
                <td>{data.Director}</td>
              </tr>
            ))}
          </tbody>
        </table>
        <div className="pagination">
          {this.state.Pagination.map(num => (
            <div
              key={num}
              className={this.state.SelectedPage === num ? 'current' : ''}
              onClick={_ => this.dispatchState({ SelectedPage: num })}
            >
              {num}
            </div>
          ))}
        </div>
      </div>
    );
  }
}
```

##### MovieDetails.js
```jsx
class MovieDetails extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    this.state = this.context.connect('MovieDetailsVM', this) || { Movie: {} };
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  render() {
    const movie = this.state.Movie || {};
    const casts = movie.Cast ? movie.Cast.split(',') : [];

    return (
      <div className="card">
        <header className="card-header">
          <b>{movie.Movie}</b>
          <div>{movie.Year}</div>
        </header>
        <section className="card-body">
          <b>Director</b>
          <p>{movie.Director}</p>
          <b>Cast</b>
          {casts.map((cast, idx) => <div key={idx}>{cast}</div>)}
        </section>
      </div>
    );
  }
}
```

##### MovieFilter.js
```jsx
class MovieFilter extends React.Component {
  static contextTypes = { connect: PropTypes.func };

  constructor(props, context) {
    super(props, context);
    // Combine state from back-end with local state.
    // This can be more concise using Object.assign if not for IE 11 support.
    var state = this.context.connect('MovieFilterVM', this) || {};
    var localState = {
      filters: [],
      filterId: 0,
      filter: 'Any',
      operation: 'contains',
      operations: [ 'contains' ],
      text: ''
    };
    for (var prop in localState) state[prop] = localState[prop];
    this.state = state;
  }

  componentWillUnmount() {
    this.vm.$destroy();
  }

  handleApply = () => {
    let newId = this.state.filterId + 1;
    let filter = {
      id: newId,
      property: this.state.filter,
      operation: this.state.operation,
      text: this.state.text
    };
    this.setState({
      filterId: newId,
      filters: [ filter, ...this.state.filters ],
      text: ''
    });
    this.dispatch({ Apply: filter });

    this.updateFilterDropdown('Any'); // Reset filter dropdown.
  };

  handleDelete = id => {
    this.dispatch({ Delete: id });
    this.setState({ filters: this.state.filters.filter(filter => filter.id != id) });
  };

  updateFilterDropdown = value => {
    this.setState({ filter: value });
    if (value == 'Rank' || value == 'Year')
      this.setState({ filter: value, operations: [ 'equals', '>=', '<=' ], operation: 'equals' });
    else this.setState({ filter: value, operations: [ 'contains' ], operation: 'contains' });
  };

  render() {
    const movieProps = [ 'Any', 'Rank', 'Movie', 'Year', 'Cast', 'Director' ];

    const filters = this.state.filters.map(filter => (
      <div className="chip" key={filter.id}>
        {filter.property} {filter.operation} {filter.text}
        <div onClick={() => this.handleDelete(filter.id)}>
          <i className="material-icons">clear</i>
        </div>
      </div>
    ));

    return (
      <form>
        <div className="filter card">
          <div className="card-header">Filters</div>
          <div className="card-body">
            <select
              className="form-control"
              value={this.state.filter}
              onChange={e => this.updateFilterDropdown(e.target.value)}
            >
              {movieProps.map((text, idx) => (
                <option key={idx} value={text}>
                  {text}
                </option>
              ))}
            </select>
            <div className="operation ">
              {this.state.operations.map((text, idx) => (
                <div key={idx} className="form-check">
                  <input
                    id={text}
                    className="form-check-input"
                    type="radio"
                    value={text}
                    checked={this.state.operation === text}
                    onChange={_ => this.setState({ operation: text })}
                  />
                  <label className="form-check-label" htmlFor={text}>
                    {text}
                  </label>
                </div>
              ))}
            </div>
            <TextBox id="FilterText" value={this.state.text} onChange={val => this.setState({ text: val })} />
            <div>{filters}</div>
          </div>
          <div className="card-footer">
            <button className="btn btn-primary" onClick={this.handleApply} disabled={!this.state.text}>
              Apply
            </button>
          </div>
        </div>
      </form>
    );
  }
}
```

##### CompositeViewVM.cs

```csharp
public class CompositeViewVM : BaseVM
{
  private readonly IMovieService _movieService;
  private event EventHandler<int> Selected;

  public CompositeViewVM(IMovieService movieService)
  {
      _movieService = movieService;
  }

  public override void OnSubVMCreated(BaseVM subVM)
  {
      if (subVM is FilterableMovieTableVM)
        InitMovieTableVM(subVM as FilterableMovieTableVM);
      else if (subVM is MovieDetailsVM)
        InitMovieDetailsVM(subVM as MovieDetailsVM);
  }

  private void InitMovieTableVM(FilterableMovieTableVM vm)
  {
      // Set the movie table data source to AFI Top 100 movies.
      vm.DataSource = () => _movieService.GetAFITop100();

      // When movie table selection changes, raise a private Selected event.
      vm.Selected += (sender, rank) => Selected?.Invoke(this, rank);
  }

  private void InitMovieDetailsVM(MovieDetailsVM vm)
  {
      // Set default details to the highest ranked movie.
      vm.SetByAFIRank(1);

      // When the Selected event occurs, update the movie details.
      Selected += (sender, rank) => vm.SetByAFIRank(rank);
  }
}
```

##### FilterableMovieTableVM.cs

```csharp
public class FilterableMovieTableVM : BaseVM
{
  private Func<IEnumerable<MovieRecord>> _dataSourceFunc;
  private Action _updateData;
  private string _query;

  public event EventHandler<int> Selected;

  public Func<IEnumerable<MovieRecord>> DataSource
  {
      set
      {
        _dataSourceFunc = value;
        _updateData?.Invoke();
      }
  }

  // This method is called when an instance of a view model inside this view model's scope is being created.
  // It provides a chance for this view model to initialize them.
  public override void OnSubVMCreated(BaseVM subVM)
  {
      if (subVM is MovieTableVM)
        InitMovieTableVM(subVM as MovieTableVM);
      else if (subVM is MovieFilterVM)
        InitMovieFilterVM(subVM as MovieFilterVM);
  }

  private void InitMovieTableVM(MovieTableVM vm)
  {
      // Forward the movie table's Selected event.
      vm.Selected += (sender, key) => Selected?.Invoke(this, key);

      // Create an action to update the movie table with the filtered data.
      _updateData = () => vm.DataSource = GetFilteredData;
      _updateData();
  }

  private void InitMovieFilterVM(MovieFilterVM vm)
  {
      // If a filter is added, set the filter query and update the movie table data.
      vm.FilterChanged += (sender, query) =>
      {
        _query = query;
        _updateData?.Invoke();
      };
  }

  private IEnumerable<MovieRecord> GetFilteredData()
  {
      try
      {
        return !string.IsNullOrEmpty(_query) ?
        _dataSourceFunc().AsQueryable().Where(_query) : _dataSourceFunc();
      }
      catch (Exception)
      {
        return new List<MovieRecord>();
      }
  }
}
```

##### MovieTableVM.cs

```csharp
public interface IPaginatedTable<T>
{
  IEnumerable<string> Headers { get; }
  IEnumerable<T> Data { get; }
  string ItemKey { get; }
  int SelectedKey { get; set; }
  int[] Pagination { get; }
  int SelectedPage { get; set; }
}

public class MovieTableVM : BaseVM, IPaginatedTable<MovieRecord>
{
  private int _recordsPerPage = 10;
  private Func<IEnumerable<MovieRecord>> _dataSourceFunc;

  public IEnumerable<string> Headers => new string[] { "Rank", "Movie", "Year", "Director" };

  public Func<IEnumerable<MovieRecord>> DataSource
  {
      set
      {
        _dataSourceFunc = value;
        Changed(nameof(Data));
      }
  }

  public IEnumerable<MovieRecord> Data => GetData();

  public string ItemKey => nameof(MovieRecord.Rank);

  public int SelectedKey
  {
      get => Get<int>();
      set
      {
        Set(value);
        Selected?.Invoke(this, value);
      }
  }

  public int[] Pagination
  {
      get => Get<int[]>();
      set
      {
        Set(value);
        SelectedPage = 1;
      }
  }

  public int SelectedPage
  {
      get => Get<int>();
      set
      {
        Set(value);
        Changed(nameof(Data));
      }
  }

  public event EventHandler<int> Selected;

  private IEnumerable<MovieRecord> GetData()
  {
      if (_dataSourceFunc == null)
        return null;

      var data = _dataSourceFunc();
      if (!data.Any(i => i.Rank == SelectedKey))
        SelectedKey = data.Count() > 0 ? data.First().Rank : -1;

      return Paginate(data);
  }

  private IEnumerable<MovieRecord> Paginate(IEnumerable<MovieRecord> data)
  {
      // ChangedProperties is a base class property that contains a list of changed properties.
      // Here it's used to check whether user has changed the SelectedPage property value by clicking a pagination button.
      if (this.HasChanged(nameof(SelectedPage)))
        return data.Skip(_recordsPerPage * (SelectedPage - 1)).Take(_recordsPerPage);
      else
      {
        var pageCount = (int)Math.Ceiling(data.Count() / (double)_recordsPerPage);
        Pagination = Enumerable.Range(1, pageCount).ToArray();
        return data.Take(_recordsPerPage);
      }
  }
}
```

##### MovieDetailsVM.cs

```csharp
public class MovieDetailsVM : BaseVM
{
  private readonly IMovieService _movieService;

  public MovieRecord Movie
  {
      get { return Get<MovieRecord>(); }
      set { Set(value); }
  }

  public MovieDetailsVM(IMovieService movieService)
  {
      _movieService = movieService;
  }

  public void SetByAFIRank(int rank) => Movie = _movieService.GetMovieByAFIRank(rank);
}
```

##### MovieFilterVM.cs

```csharp
public class MovieFilterVM : BaseVM
{
  private List<MovieFilter> _filters = new List<MovieFilter>();

  public class MovieFilter
  {
      public int Id { get; set; }
      public string Property { get; set; }
      public string Operation { get; set; }
      public string Text { get; set; }

      public string ToQuery()
      {
        if (Operation == "contains")
            return Property == "Any" ? $"( Movie + Cast + Director ).toLower().contains(\"{Text.ToLower()}\")"
              : $"{Property}.toLower().contains(\"{Text.ToLower()}\")";
        else
        {
            int intValue = int.Parse(Text);
            if (Operation == "equals")
              return $"{Property} == {intValue}";
            else
              return $"{Property} {Operation} {intValue}";
        }
      }

      public static string BuildQuery(IEnumerable<MovieFilter> filters) => string.Join(" and ", filters.Select(i => i.ToQuery()));
  }

  public Action<MovieFilter> Apply => arg =>
  {
      _filters.Add(arg);
      FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
  };

  public Action<int> Delete => id =>
  {
      _filters = _filters.Where(i => i.Id != id).ToList();
      FilterChanged?.Invoke(this, MovieFilter.BuildQuery(_filters));
  };

  public event EventHandler<string> FilterChanged;
}
```
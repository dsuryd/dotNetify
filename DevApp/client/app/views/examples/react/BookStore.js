import React from 'react';
import dotnetify from 'dotnetify';
import { RouteLink } from 'dotnetify/react';
import { Modal, Theme } from 'dotnetify-elements';
import { BookStoreCss, BookCss } from '../components/css';

export default class BookStore extends React.Component {
	constructor(props) {
		super(props);
		this.state = {};

		this.vm = dotnetify.react.connect('BookStoreVM', this);
		this.vm.onRouteEnter = (path, template) => (template.Target = 'BookPanel');
	}

	componentWillUnmount() {
		this.vm.$destroy();
	}

	render() {
		return (
			<BookStoreCss>
				<header>
					Each product here is represented by a unique URL that can be entered into the address bar to go directly to that
					specific product page.
				</header>
				<BookStoreFront vm={this.vm} books={this.state.Books} />
				<div id="BookPanel" />
			</BookStoreCss>
		);
	}
}

const BookStoreFront = ({ vm, books }) => {
	if (!books) return <div />;
	return (
		<section>
			{books.map((book) => (
				<div key={book.Info.Id}>
					<center>
						<RouteLink vm={vm} route={book.Route}>
							<img className="thumbnail" src={book.Info.ImageUrl} />
						</RouteLink>
						<div>
							<b>{book.Info.Title}</b>
							<div>{'by ' + book.Info.Author}</div>
						</div>
					</center>
				</div>
			))}
		</section>
	);
};

export const BookDefault = () => <div />;

export class Book extends React.Component {
	constructor(props) {
		super(props);
		this.vm = dotnetify.react.connect('BookDetailsVM', this);
		this.state = { Book: { Title: '', ImageUrl: '', Author: '', ItemUrl: '' }, open: true };
	}

	componentWillUnmount() {
		this.vm && this.vm.$destroy();
	}

	handleClose = () => {
		this.setState({ open: false });
		this.vm.$destroy();
		this.vm = null;
		window.history.back();
	};

	render() {
		const book = this.state.Book;
		return (
			<Theme>
				<Modal open={this.state.open}>
					<BookCss>
						<img className="thumbnail" src={book.ImageUrl} />
						<div>
							<h3>{book.Title}</h3>
							<h5>{book.Author}</h5>
							<button className="btn btn-primary">Buy</button>
						</div>
					</BookCss>
					<footer>
						<button className="btn btn-success" onClick={this.handleClose}>
							Back
						</button>
					</footer>
				</Modal>
			</Theme>
		);
	}
}

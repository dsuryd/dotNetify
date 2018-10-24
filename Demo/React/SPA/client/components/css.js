import styled from 'styled-components';

export const SimpleListCss = styled.div`
	padding: 0 1rem;
	header {
		display: flex;
		align-items: center;
		margin-bottom: 1rem;
		> * {
			margin-right: 1rem;
		}
		input[type="text"] {
			max-width: 15rem;
		}
	}
	table {
		font-size: unset;
		width: 100%;
		max-width: 1268px;
		td,
		th {
			padding: 0.5rem 0;
			padding-right: 2rem;
			border-bottom: 1px solid #ddd;
			width: 50%;
		}
		th {
			font-weight: 500;
		}
		td:last-child,
		th:last-child {
			width: 5rem;
			> div {
				display: flex;
				align-items: center;
				cursor: pointer;
			}
		}
		tr:hover {
			background: #efefef;
		}
		i.material-icons {
			font-size: 1.2rem;
		}
		span.editable:hover {
			&:after {
				font-family: "Material Icons";
				content: "edit";
			}
		}
	}
`;

export const LiveChartCss = styled.div`
	padding: 0 1rem;
	width: 100%;
	max-width: 1268px;
	> div:first-child {
		display: inline-block;
		width: 70%;
	}
	> div:last-child {
		display: inline-block;
		width: 30%;
		> *:last-child {
			margin-top: 2rem;
		}
	}
	@media (max-width: 1170px) {
		> div:first-child,
		> div:last-child {
			display: block;
			width: 100%;
			margin-bottom: 2rem;
		}
	}
`;

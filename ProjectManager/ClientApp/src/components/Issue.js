import React, {Component} from 'react';
import {request} from "../requests";
import {Badge, Button, Col, Form, FormGroup, NavLink, Row} from "reactstrap";
import {Link, Redirect} from "react-router-dom";

export class Issue extends Component {
    static displayName = Issue.name;

    state = {
        goBack: false
    }

    componentDidMount = () => {
        this.setCurrentIssue(this.props.match.params.id);
    }

    componentWillReceiveProps = (nextProps) => {
        this.setCurrentIssue(nextProps.match.params.id)
    }

    setCurrentIssue = async (id) => {
        const issueResponse = await request('issues/' + id);
        if (issueResponse.status !== 200) {
            this.setState({
                goBack: true
            })
            return;
        }
        const issue = await issueResponse.json();

        this.setState({
            issue
        });

        if (issue && issue.parent) {
            const parent = await this.fetchIssue(issue.parent);
            this.setState({
                parent
            });
        } else {
            this.setState({
                parent: null
            });
        }

        if (issue && issue.children && issue.children.length > 0) {
            const children = [];
            for (const childId of issue.children) {
                const child = await this.fetchIssue(childId);
                children.push(child);
            }
            this.setState({
                children
            });
        } else {
            this.setState({
                children: null
            });
        }
    }

    fetchIssue = async (id) => {
        const issueResponse = await request('issues/' + id);
        if (issueResponse.status !== 200) {
            this.setState({
                goBack: true
            })
            return;
        }
        return await issueResponse.json();
    }

    render() {
        const children = this.state.children
            ? this.childrenList(this.state.children)
            : '';

        return (
            <div>
                {this.state.goBack ? <Redirect to="/"/> : ''}
                <p>{this.parentIssue(this.state.parent)}</p>
                <h2>{this.state.issue ? this.state.issue.type : ''} #{this.state.issue ? this.state.issue.id : ''}</h2>
                <h3>{this.state.issue ? this.state.issue.name : ''}</h3>
                <Badge color="primary">{this.state.issue ? this.state.issue.status : ''} </Badge>
                <p>{this.state.issue ? this.state.issue.description : ''}</p>

                <Row>
                    <Col>Project: {this.projectLink(this.state.issue)}</Col>
                    <Col>Assignee: {this.assigneeLink(this.state.issue)}</Col>
                    <Col>Estimated: {this.estimated(this.state.issue)}</Col>
                </Row>

                <p></p>
                {children}
            </div>
        );
    }

    parentIssue = (issue) => {
        if (issue) {
            return <Link tag={Link} className="link-nopadding"
                         to={`/view/issue/${issue.id}`}>Parent {issue.type} #{issue.id}: {issue.project.name}</Link>;
        } else {
            return '';
        }
    }

    projectLink = (issue) => {
        if (issue) {
            return <NavLink tag={Link} className="link-nopadding"
                            to={`/view/project/${issue.project.id}`}>{issue.project.name}</NavLink>;
        } else {
            return '';
        }
    }

    assigneeLink = (issue) => {
        if (issue) {
            return <NavLink tag={Link} className="link-nopadding"
                            to={`/view/user/${issue.assignee.id}`}>{issue.assignee.name}</NavLink>;
        } else {
            return '';
        }
    }

    estimated = (issue) => {
        if (issue) {
            return <div>
                {issue.estimateHours ? issue.estimateHours : 'N/A'} Hours
            </div>
        } else {
            return '';
        }
    }

    childrenList = (children) => {
        if (children) {
            const list = children.map(child =>
                <li key={"child-issue-" + child.id}>
                    <Link tag={Link}
                          className="link-nopadding"
                          to={`/view/issue/${child.id}`}>
                        {child.type} #{child.id}: {child.name}
                        <Badge style={{ 'margin-left': '0.5em' }} color="secondary">{child.status}</Badge>
                    </Link>
                </li>
            );
            return <div>
                <h5>Children Issues</h5>
                <ul>
                    {list}
                </ul>
            </div>
        } else {
            return '';
        }
    }
}

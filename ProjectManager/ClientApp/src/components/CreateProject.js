import React, {Component} from 'react';
import {Button, Form, FormFeedback, FormGroup, Input, Label, NavLink} from "reactstrap";
import {Link, Redirect} from "react-router-dom";
import {request} from "../requests";

export class CreateProject extends Component {
    static displayName = CreateProject.name;

    state = {
        name: '',
        hourRate: 0,
        nameValid: true,
        nameFeedback: '',
        hourRateValid: true,
        hourRateFeedback: '',
        submit: false
    }

    render() {
        return (
            <div>
                <h1>New project creation</h1>
                <p>Use this form to create a new project.
                    Specify name of the project and a hour rate, which will be used for price evaluation.
                </p>
                <Form onSubmit={this.create}>
                    <FormGroup>
                        <Label for="name">Name of project</Label>
                        <Input type="text"
                               name="name"
                               id="name"
                               value={this.state.name}
                               onChange={this.setName}
                               invalid={!this.state.nameValid}
                        />
                        <FormFeedback>{this.state.nameFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Label for="hourRate">Hour rate</Label>
                        <Input
                            type="number"
                            name="hourRate"
                            id="hourRate"
                            placeholder="Hour rate"
                            value={this.state.hourRate}
                            onChange={this.setHourRate}
                            invalid={!this.state.hourRateValid}
                        />
                        <FormFeedback>{this.state.hourRateFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup inline>
                        <Button color="primary" onClick={this.create}>Create project</Button>
                        <NavLink tag={Link} style={{display: 'inline-block'}} to="/">
                            <Button color="primary" outline onClick={this.create}>Cancel</Button>
                        </NavLink>
                    </FormGroup>
                    {this.state.submit ? <Redirect to="/"/> : ''}
                </Form>
            </div>
        );
    }

    setName = (e) => {
        this.setState({
            name: e.target.value,
            nameValid: true
        });
    }

    setHourRate = (e) => {
        const number = e.target.value;
        this.setState({
            hourRate: parseInt(number),
            hourRateValid: true
        });
    }

    create = async () => {
        let hourRate = parseInt(this.state.hourRate)
        if (hourRate == null || Number.isNaN(hourRate)) {
            hourRate = 0
        }
        
        const response = await request('projects', 'POST', {
            'Name': this.state.name,
            'HourValue': hourRate
        });

        const body = await response.json();
        if (response.status === 200) {
            await request('projects/' + body.id, 'POST')
            this.setState({
                submit: true
            })
            return;
        }

        if (body.message.includes('Name')) {
            this.setState({
                nameValid: false,
                nameFeedback: body.message
            });
        }
        if (body.message.includes('Hour')) {
            this.setState({
                hourRateValid: false,
                hourRateFeedback: body.message
            })
        }
    }
}

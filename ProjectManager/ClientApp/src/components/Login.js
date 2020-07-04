import React, {Component} from 'react';
import './Login.css'
import {Button, Container, Form, FormFeedback, FormGroup, Input} from "reactstrap";
import {request} from "../requests";

export class Login extends Component {
    static displayName = Login.name;

    state = {
        login: '',
        password: '',
        loginValid: true,
        loginFeedback: '',
        passwordValid: true,
        passwordFeedback: ''
    }

    render() {
        return (
            <Container className={"login-container"}>
                <h1>Login</h1>
                <p>Login or register to continue.</p>
                <Form onSubmit={this.login}>
                    <FormGroup>
                        <Input type="text"
                               name="login"
                               id="login"
                               placeholder="Login"
                               value={this.state.login}
                               onChange={this.setLogin}
                               invalid={!this.state.loginValid}
                        />
                        <FormFeedback>{this.state.loginFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup>
                        <Input
                            type="password"
                            name="password"
                            id="password"
                            placeholder="Hasło"
                            value={this.state.password}
                            onChange={this.setPassword}
                            invalid={!this.state.passwordValid}
                        />
                        <FormFeedback>{this.state.passwordFeedback}</FormFeedback>
                    </FormGroup>
                    <FormGroup inline>
                        <Button color="primary" outline onClick={this.login}>Login</Button>
                        <Button color="link" onClick={this.register}>Register</Button>
                    </FormGroup>
                </Form>
            </Container>
        );
    }

    setLogin = (e) => {
        this.setState({
            login: e.target.value,
            loginValid: true,
            passwordValid: true
        })
    }

    setPassword = (e) => {
        this.setState({
            password: e.target.value,
            loginValid: true,
            passwordValid: true
        })
    }

    login = async () => {
        const response = await request('authentication', 'POST', {
            'Name': this.state.login,
            'Password': this.state.password
        });
        const status = response.status;
        const successfulLogin = status === 200;
        this.setState({
            loginValid: successfulLogin,
            passwordValid: successfulLogin
        });
        if (successfulLogin && this.props.onLogin) {
            return this.props.onLogin();
        } else {
            const body = await response.json();
            this.setState({
                passwordFeedback: body.message
            });
        }
    }
    
    register = async () => {
        const response = await request('user', 'POST', {
            'Name': this.state.login,
            'Password': this.state.password
        });
        const status = response.status;
        
        if (status === 200) {
            return await this.login();
        }
        
        const body = await response.json()
        
        if (body.message && (body.message.includes('exists') || body.message.includes('Name'))) {
            this.setState({
                loginValid: false,
                loginFeedback: body.message
            });
        } else {
            this.setState({
                passwordValid: false,
                passwordFeedback: body.message
            })
        }
    }
}

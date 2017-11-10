import React, { Component } from 'react'
import {
  BrowserRouter as Router,
  Route
} from 'react-router-dom'

import Navbar from './components/Navbar'
import Container from './components/Container'
import Row from './components/Row'
import Column from './components/Column'

import Home from './Home/Home'
import './App.css'

export default class App extends Component {
  render() {
    return (
      <Router>
        <Container>
          <Navbar/>
          <Row>
            <Column>
              <Route exact path="/" component={Home}/>
            </Column>
          </Row>
        </Container>
      </Router>
    )
  }
}

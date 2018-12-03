import * as React from 'react';
import { Route } from 'react-router';
import { Home } from './components/Home';
import { Layout } from './components/Layout';

export default class App extends React.Component {
  public render() {
    return (
      <Layout>
        <Route path='/' component={Home} />
      </Layout>
    );
  }
}

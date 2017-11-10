import React from 'react'

const Header = ({ children }) => (
  <thead>
    {children}
  </thead>
)

const Body = ({ children }) => (
  <tbody>
    {children}
  </tbody>
)

const Table = ({ children }) => (
  <table className="table">
    {children}
  </table>
)

Table.Header = Header
Table.Body = Body

export default Table

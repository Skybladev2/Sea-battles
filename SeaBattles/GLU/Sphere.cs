// взято откуда-то с геймдева

//void pgGlobe::CreateSphere()
//{
//  int n = 0;
//  const float m_PI_180 = M_PI / 180.0f;

//  for( double b = 0; b <= 90 - m_space; b+=m_space )
//  {

//    for( double a = 0; a <= 360 - m_space; a+=m_space)
//    {

//      m_vertices[n].X = m_radius * sin( a * m_PI_180 ) * sin( b * m_PI_180 );
//      m_vertices[n].Y = m_radius * cos( a * m_PI_180 ) * sin( b * m_PI_180 );
//      m_vertices[n].Z = m_radius * cos( b * m_PI_180 );

//      m_vertices[n].X = m_radius * sin( a * m_PI_180 ) * sin( (b + m_space) * m_PI_180 );
//      m_vertices[n].Y = m_radius * cos( a * m_PI_180 ) * sin( (b + m_space) * m_PI_180 );
//      m_vertices[n].Z = m_radius * cos( (b + m_space ) * m_PI_180);

//      m_vertices[n].X = m_radius * sin((a + m_space) * m_PI_180) * sin( b * m_PI_180 );
//      m_vertices[n].Y = m_radius * cos((a + m_space) * m_PI_180) * sin( b * m_PI_180 );
//      m_vertices[n].Z = m_radius * cos((b) * m_PI_180);

//      m_vertices[n].X = m_radius * sin((a + m_space) * m_PI_180) * sin( (b + m_space) * m_PI_180 );
//      m_vertices[n].Y = m_radius * cos((a + m_space) * m_PI_180) * sin( (b + m_space) * m_PI_180 );
//      m_vertices[n].Z = m_radius * cos((b + m_space) * m_PI_180);  
//    }
//  }

//  m_vertices[n] = m_vertices[0];  
//  m_VertexCount = n;
//}


//вот рисовалка 
//void displaySphere()
//{

//  int b;
  
//  glBindTexture (GL_TEXTURE_2D, m_texture);
//  glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_LINEAR );
//  glTexParameterf( GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_LINEAR );

//  glBegin (GL_TRIANGLE_STRIP);

//  for ( b = 0; b <= m_VertexCount; b++)
//  {
//    glTexCoord2f (m_vertices[b].U, m_vertices[b].V);
//    glVertex3f (m_vertices[b].X, m_vertices[b].Y, -m_vertices[b].Z);
//  }

//  for ( b = 0; b <= m_VertexCount; b++)
//  {
//    glTexCoord2f( m_vertices[b].U, -m_vertices[b].V );
//    glVertex3f( m_vertices[b].X, m_vertices[b].Y, m_vertices[b].Z );
//  }

//  glEnd();
//}
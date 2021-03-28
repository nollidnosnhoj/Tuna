import Axios from 'axios';


export async function revokeRefreshToken() {
  return await Axios.post('/api/auth/revoke', undefined, {
    withCredentials: true,
    validateStatus: (status) => status < 500
  });
}

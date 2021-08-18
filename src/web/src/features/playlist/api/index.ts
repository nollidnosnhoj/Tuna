import request from "~/lib/http";

export async function checkIfPlaylistFavoritedRequest(
  playlistId: string
): Promise<boolean> {
  try {
    const res = await request({
      method: "head",
      url: `me/favorites/playlists/${playlistId}`,
      validateStatus: (status) => {
        return status === 404 || status < 400;
      },
    });

    return res.status !== 404;
  } catch (err) {
    return false;
  }
}

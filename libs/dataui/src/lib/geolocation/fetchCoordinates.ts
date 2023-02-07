const coordsQuery = `
{
  configuration {
    area {
      topLeft {
        latitude
        longitude
      }
      bottomRight {
        latitude
        longitude
      }
      center {
        latitude
        longitude
      }
    }
  }
}
`;

interface CoordsQueryResult {
  configuration: {
    area: {
      topLeft: {
        latitude: number;
        longitude: number;
      };
      bottomRight: {
        latitude: number;
        longitude: number;
      };
      center: {
        latitude: number;
        longitude: number;
      };
    };
  };
}

export async function fetchCoordinates() {
  const response = await fetch('/graphql', {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json',
    },
    body: JSON.stringify({
      query: coordsQuery,
      variables: {},
    }),
  });

  if (!response.ok) {
    throw new Error(
      `Error response from server ${response.status} ${response.statusText}`
    );
  }

  return (await response.json()) as {
    data: CoordsQueryResult;
  };
}

interface Props {
  transportId: string;
}

export function TransportView(props: Props) {
  const { transportId } = props;
  return (
    <div>
      <h2>Transport ({transportId})</h2>
    </div>
  )
}

export default TransportView;
